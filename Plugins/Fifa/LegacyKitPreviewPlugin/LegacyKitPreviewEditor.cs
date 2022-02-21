using Frosty.Core.Controls;
using Frosty.Core.Controls.Editors;
using Frosty.Core.Windows;
using Frosty.Core.Viewport;
using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using DXUT = Frosty.Core.Viewport.DXUT;
using Frosty.Core.Legacy;
using Frosty.Core;
using Frosty.Hash;
using LegacyDatabasePlugin.IO;
using LegacyDatabasePlugin.Database;
using Frosty.Core.Misc;
using ImageSource = System.Windows.Media.ImageSource;
using MeshSetPlugin.Screens;
using MeshSetPlugin.Render;
using MeshSetPlugin.Resources;

namespace LegacyKitPreviewPlugin
{
    public enum FifaKitTechType
    {
        Home,
        Away,
        Goalie
    }

    public class FrostyFifaTeamNameDataEditor : FrostyCustomComboDataEditor<int, string>
    {
    }

    public class FifaKitPropertyInfo
    {
        [EbxFieldMeta(EbxFieldType.Struct)]
        [Editor(typeof(FrostyFifaTeamNameDataEditor))]
        public CustomComboData<int, string> Team { get; set; }
        [EbxFieldMeta(EbxFieldType.Enum)]
        public FifaKitTechType Type { get; set; }
        [EbxFieldMeta(EbxFieldType.Int32)]
        public int JerseyNumber { get; set; } = 12;

        [Category("Preview")]
        [EbxFieldMeta(EbxFieldType.Boolean)]
        public bool IsFemale { get; set; } = false;
    }

    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyViewport))]
    [TemplatePart(Name = PART_PropertyGrid, Type = typeof(FrostyPropertyGrid))]
    public class LegacyKitPreviewEditor : FrostyBaseEditor
    {
        private const string PART_Renderer = "PART_Renderer";
        private const string PART_PropertyGrid = "PART_PropertyGrid";

        public override ImageSource Icon => LegacyKitPreviewMenuExtension.imageSource;

        private MultiMeshPreviewScreen screen = new MultiMeshPreviewScreen();
        private FrostyViewport renderer;
        private FrostyPropertyGrid pg;
        private bool firstTimeLoad = true;
        private List<Tuple<string, MeshSet, MeshMaterialCollection>> meshes = new List<Tuple<string, MeshSet, MeshMaterialCollection>>();

        private List<int> renderMeshes = new List<int>();

        private FifaKitPropertyInfo info;
        private Dictionary<int, Dictionary<int, LegacyDbRow>> rows = new Dictionary<int, Dictionary<int, LegacyDbRow>>();
        private LegacyDb database;
        private int selectedValue = 0;
        private ILogger logger;

        private string[] meshPaths = new string[]
        {
            "content/character/body/body_simhigh_0/jersey_0_adidaschippedcenter1_long_noband_tucked_regular_<GenderString>/jersey_0_13_1_0_0_0_<GenderInt>_mesh",
            "content/character/body/body_simhigh_0/shorts_regular_<GenderString>/shorts_0_0_<GenderInt>_mesh",
            "content/character/body/body_simhigh_0/socks_regular_long_<GenderString>/sock_0_2_<GenderInt>_mesh",
            "content/character/body/body_simhigh_0/body_legs_reg_long_socks_<GenderString>/legs_0_2_<GenderInt>_mesh",
            "content/character/body/body_simhigh_0/body_arms_long_sleeve_average_<GenderString>/arms_0_1_<GenderInt>_mesh",
            "content/character/shoe/shoe_0/shoe_0_mesh",
            "content/character/player/player_<Head>"
        };

        static LegacyKitPreviewEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegacyKitPreviewEditor), new FrameworkPropertyMetadata(typeof(LegacyKitPreviewEditor)));
        }

        public LegacyKitPreviewEditor(ILogger inLogger)
        {
            logger = inLogger;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            renderer = GetTemplateChild(PART_Renderer) as FrostyViewport;
            pg = GetTemplateChild(PART_PropertyGrid) as FrostyPropertyGrid;
            Loaded += FifaKitPreviewEditor_Loaded;

            renderer.Screen = screen;
            screen.camera = new DXUT.ModelViewerCamera();
            screen.SunPosition = new Vector3(-20.0f, 20.0f, 20.0f);
        }

        private void FifaKitPreviewEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                FrostyTaskWindow.Show("Loading database", "", (task) =>
                {
                    // load in main database
                    LegacyFileEntry dbEntry = App.AssetManager.GetCustomAssetEntry<LegacyFileEntry>("legacy", "data/db/fifa_ng_db.db");
                    LegacyFileEntry metaEntry = App.AssetManager.GetCustomAssetEntry<LegacyFileEntry>("legacy", "data/db/fifa_ng_db-meta.xml");

                    using (LegacyDbReader reader = new LegacyDbReader(App.AssetManager.GetCustomAsset("legacy", metaEntry), App.AssetManager.GetCustomAsset("legacy", dbEntry)))
                        database = reader.ReadDb();
                });

                info = new FifaKitPropertyInfo();
                LegacyDbTable teamTable = database["teams"];
                LegacyDbTable teamKitTable = database["teamkits"];

                List<string> names = new List<string>();
                List<int> values = new List<int>();

                foreach (LegacyDbRow row in teamTable.Rows)
                {
                    names.Add((string)row["teamname"]);
                    values.Add((int)row["teamid"]);
                }
                foreach (LegacyDbRow row in teamKitTable.Rows)
                {
                    int teamId = (int)row["teamtechid"];
                    int kitTechId = (int)row["teamkittypetechid"];
                    if (!rows.ContainsKey(teamId))
                        rows.Add(teamId, new Dictionary<int, LegacyDbRow>());
                    if (!rows[teamId].ContainsKey(kitTechId))
                        rows[teamId].Add(kitTechId, row);
                }

                var joined = values.Zip(names, (a, b) => new { Name = b, Value = a })
                                   .OrderBy(x => x.Name);

                info.Team = new CustomComboData<int, string>(joined.Select(x => x.Value).ToList(), joined.Select(x => x.Name).ToList());
                info.Type = FifaKitTechType.Home;

                pg.SetClass(info);
                pg.OnModified += Pg_OnModified;

                // load in meshes
                RefreshMeshes();

                firstTimeLoad = false;
            }
        }

        private void Pg_OnModified(object sender, ItemModifiedEventArgs e)
        {
            if (e.Item.Name == "IsFemale")
            {
                RefreshMeshes();
                return;
            }
            
            RefreshTextures();
        }

        private Tuple<string, MeshSet, MeshMaterialCollection> LoadMesh(string name, int gender)
        {
            string genderString = (gender == 0) ? "male" : "female";
            string headPath = (gender == 0) ? "20500/cristiano_ronaldo_20801/var_0/head_20801_0_0_mesh" : "226000/christine_sinclair_226359/var_0/head_226359_0_0_mesh";
            if (ProfilesLibrary.DataVersion == 0)
                headPath = (gender == 0) ? "20500/cristiano_ronaldo_20801/head_20801_0_mesh" : "226000/christine_sinclair_226359/head_226359_0_mesh";

            name = name.Replace("<GenderString>", genderString);
            name = name.Replace("<GenderInt>", gender.ToString());
            name = name.Replace("<Head>", headPath);

            EbxAsset asset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(name));
            MeshSet meshSet = App.AssetManager.GetResAs<MeshSet>(App.AssetManager.GetResEntry((ulong)((dynamic)asset.RootObject).MeshSetResource));
            return new Tuple<string, MeshSet, MeshMaterialCollection>(name, meshSet, new MeshMaterialCollection(asset, new PointerRef()));
        }

        private void RefreshTextures()
        {
            //if (selectedValue == info.Team.SelectedValue)
            //    return;

            LoadKitAssets(info);
            if (renderMeshes.Count > 0)
            {
                for (int i = 0; i < meshes.Count; i++)
                    screen.LoadMaterials(renderMeshes[i], meshes[i].Item3);
            }
        }

        private void RefreshMeshes()
        {
            Dispose();
            foreach (string meshPath in meshPaths)
            {
                meshes.Add(LoadMesh(meshPath, info.IsFemale ? 1 : 0));
                renderMeshes.Add(screen.AddMesh(meshes[meshes.Count - 1].Item2, meshes[meshes.Count - 1].Item3, Matrix.Identity, LoadSkeleton("content/character/rig/skeleton/player/skeleton_player")));
            }

            screen.SetAnimation(LoadAnim("Resources\\FifaKitPreviewStance.dat"));
            RefreshTextures();
        }

        private void LoadKitAssets(FifaKitPropertyInfo info)
        {
            selectedValue = info.Team.SelectedValue;

            int teamId = selectedValue;
            int teamTechId = (int)info.Type;
            int jerseyNumber = info.JerseyNumber;

            if (!rows.ContainsKey(teamId))
                return;

            if (jerseyNumber < 0) jerseyNumber = 0;
            else if (jerseyNumber > 99) jerseyNumber = 99;

            int jerseyNumberTens = jerseyNumber / 10;
            int jerseyNumberOnes = jerseyNumber % 10;

            LegacyDbRow row = rows[teamId][teamTechId];

            string pattern = teamId.ToString() + "/";
            switch (teamTechId)
            {
                case 0: pattern += "home_0_0"; break;
                case 1: pattern += "away_1_0"; break;
                case 2: pattern += "goalie_home_2_0"; break;
            }

            string jerseyPattern = pattern + "/jersey_" + teamId.ToString() + "_" + teamTechId.ToString() + "_0_";
            string shortsPattern = pattern + "/shorts_" + teamId.ToString() + "_" + teamTechId.ToString() + "_0_";
            string socksPattern = pattern + "/socks_" + teamId.ToString() + "_" + teamTechId.ToString() + "_0_";
            string crestPattern = pattern + "/crest_" + teamId.ToString() + "_" + teamTechId.ToString() + "_0_";
            string hotspotsPattern = pattern + "/hotspots_" + teamId.ToString() + "_" + teamTechId.ToString() + "_0";
            string numbersPattern = "numbers_" + row["numberfonttype"].ToString() + "_";
            bool hasFrontNumber = row["jerseyfrontnumberplacementcode"].ToString() == "1";

            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx(type: "TextureAsset"))
            {
                if (entry.Name.Contains(jerseyPattern))
                {
                    if (entry.Name.EndsWith("color")) meshes[0].Item3.SetTextureParameter("colorTexture", entry);
                    else if (entry.Name.EndsWith("normal")) meshes[0].Item3.SetTextureParameter("normalTexture", entry);
                    else meshes[0].Item3.SetTextureParameter("coefficientTexture", entry);
                }
                else if (entry.Name.Contains(shortsPattern))
                {
                    if (entry.Name.EndsWith("color")) meshes[1].Item3.SetTextureParameter("colorTexture", entry);
                    else if (entry.Name.EndsWith("normal")) meshes[1].Item3.SetTextureParameter("normalTexture", entry);
                    else meshes[1].Item3.SetTextureParameter("coefficientTexture", entry);
                }
                else if (entry.Name.Contains(socksPattern))
                {
                    if (entry.Name.EndsWith("color")) meshes[2].Item3.SetTextureParameter("colorTexture", entry);
                    else if (entry.Name.EndsWith("normal")) meshes[2].Item3.SetTextureParameter("normalTexture", entry);
                    else meshes[2].Item3.SetTextureParameter("coefficientTexture", entry);
                }
                else if (entry.Name.Contains(crestPattern))
                    meshes[0].Item3.SetTextureParameter("crestTexture", entry);
                else if (entry.Name.Contains(numbersPattern))
                {
                    if (entry.Name.EndsWith(jerseyNumberTens + "_color"))
                    {
                        if (jerseyNumber >= 10)
                            meshes[0].Item3.SetTextureParameter("numberTensTexture", entry);
                        else
                            meshes[0].Item3.SetTextureParameter("numberTensTexture", null);
                    }
                    if (entry.Name.EndsWith(jerseyNumberOnes + "_color"))
                        meshes[0].Item3.SetTextureParameter("numberUnitsTexture", entry);
                }
            }
            foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx(type: "PlayerHotspotDataAsset"))
            {
                if (entry.Name.Contains(hotspotsPattern))
                {
                    EbxAsset hotspotsAsset = App.AssetManager.GetEbx(entry);
                    dynamic hotspots = hotspotsAsset.RootObject;

                    meshes[0].Item3.SetVectorParameter("debugCrestHotspot", hotspots.Jersey.Team);

                    Vector4 hotspot = SharpDXUtils.FromVec4(hotspots.Jersey.NumberBack);
                    if (jerseyNumber < 10)
                    {
                        float halfX = (hotspot.Z - hotspot.X) * 0.25f;
                        hotspot.X -= halfX;
                        hotspot.Z -= halfX;
                    }
                    meshes[0].Item3.SetVectorParameter("debugNumberBackHotspot", hotspot.X, hotspot.Y, hotspot.Z, hotspot.W);
                    if (hasFrontNumber)
                    {
                        hotspot = SharpDXUtils.FromVec4(hotspots.Jersey.NumberFront);
                        if (jerseyNumber < 10)
                        {
                            float halfX = (hotspot.Z - hotspot.X) * 0.25f;
                            hotspot.X -= halfX;
                            hotspot.Z -= halfX;
                        }
                        meshes[0].Item3.SetVectorParameter("debugNumberFrontHotspot", hotspot.X, hotspot.Y, hotspot.Z, hotspot.W);
                    }
                    else
                        meshes[0].Item3.SetVectorParameter("debugNumberFrontHotspot", 0, 0, 0, 0);
                }
            }

            meshes[0].Item3.SetVectorParameter("debugJerseyNumberColorPrimary", (int)row["jerseynumbercolorprimr"] / 255.0f, (int)row["jerseynumbercolorprimg"] / 255.0f, (int)row["jerseynumbercolorprimb"] / 255.0f, 1.0f);
            meshes[0].Item3.SetVectorParameter("debugJerseyNumberColorSecondary", (int)row["jerseynumbercolorsecr"] / 255.0f, (int)row["jerseynumbercolorsecg"] / 255.0f, (int)row["jerseynumbercolorsecb"] / 255.0f, 1.0f);
            meshes[0].Item3.SetVectorParameter("debugJerseyNumberColorTertiary", (int)row["jerseynumbercolorterr"] / 255.0f, (int)row["jerseynumbercolorterg"] / 255.0f, (int)row["jerseynumbercolorterb"] / 255.0f, 1.0f);
        }

        private MeshRenderSkeleton LoadSkeleton(string name)
        {
            MeshRenderSkeleton skeleton = new MeshRenderSkeleton();

            dynamic skeletonAsset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(name)).RootObject;
            dynamic boneNames = skeletonAsset.BoneNames;
            dynamic pose = skeletonAsset.ModelPose;
            dynamic localPose = skeletonAsset.LocalPose;

            for (int boneIdx = 0; boneIdx < boneNames.Count; boneIdx++)
            {
                string boneName = boneNames[boneIdx];
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
                    boneName = Murmur2.HashString64(boneName, 0x4eb23).ToString("X16");

                boneName = boneName.ToLower();
                Matrix boneMatrix = SharpDXUtils.FromLinearTransform(localPose[boneIdx]);

                Matrix mp = SharpDXUtils.FromLinearTransform(pose[boneIdx]);
                mp.Invert();

                boneName = boneNames[boneIdx];
                boneName = boneName.ToLower();

                skeleton.AddBone(new MeshRenderSkeleton.Bone()
                {
                    NameHash = Fnv1.HashString(boneName),
                    ModelPose = mp,
                    LocalPose = boneMatrix,
                    ParentBoneId = skeletonAsset.Hierarchy[boneIdx]
                });
            }

            return skeleton;
        }

        private MeshRenderAnim LoadAnim(string name)
        {
            MeshRenderAnim anim = null;
            using (NativeReader reader = new NativeReader(new FileStream(name, FileMode.Open, FileAccess.Read)))
            {
                int frameCount = reader.ReadInt();
                int keyFrameCount = reader.ReadInt();

                anim = new MeshRenderAnim(frameCount);
                Dictionary<string, MeshRenderAnim.Bone> bones = new Dictionary<string, MeshRenderAnim.Bone>();

                for (int i = 0; i < keyFrameCount; i++)
                {
                    int frameTime = reader.ReadInt();
                    int boneCount = reader.ReadInt();

                    for (int j = 0; j < boneCount; j++)
                    {
                        string boneName = reader.ReadNullTerminatedString().ToLower();
                        int type = reader.ReadInt();

                        if (!bones.ContainsKey(boneName))
                            bones.Add(boneName, new MeshRenderAnim.Bone() { NameHash = Fnv1.HashString(boneName) });

                        switch (type)
                        {
                            case 0x0E:
                                {
                                    Quaternion q = new Quaternion()
                                    {
                                        X = reader.ReadFloat(),
                                        Y = reader.ReadFloat(),
                                        Z = reader.ReadFloat(),
                                        W = reader.ReadFloat()
                                    };
                                    bones[boneName].Rotations.Add(new MeshRenderAnim.Keyframe<Quaternion>() { FrameTime = frameTime, Value = q });
                                }
                                break;

                            case 0x0F:
                                reader.ReadFloat();
                                break;

                            case 0x7a2e5497:
                                {
                                    Vector3 t = new Vector3()
                                    {
                                        X = reader.ReadFloat(),
                                        Y = reader.ReadFloat(),
                                        Z = reader.ReadFloat()
                                    };
                                    bones[boneName].Translations.Add(new MeshRenderAnim.Keyframe<Vector3>() { FrameTime = frameTime, Value = t });
                                }
                                break;

                            case 0x7a2e53c6:
                                {
                                    Vector3 s = new Vector3()
                                    {
                                        X = reader.ReadFloat(),
                                        Y = reader.ReadFloat(),
                                        Z = reader.ReadFloat()
                                    };
                                    bones[boneName].Scales.Add(new MeshRenderAnim.Keyframe<Vector3>() { FrameTime = frameTime, Value = s });
                                }
                                break;
                        }
                    }
                }

                anim.AddBones(bones.Values);
            }
            return anim;
        }

        public void Dispose()
        {
            foreach (int id in renderMeshes)
                screen.RemoveMesh(id);

            renderMeshes.Clear();
            meshes.Clear();
        }

        public override void Closed()
        {
            Dispose();
            renderer.Shutdown();
        }
    }
}
