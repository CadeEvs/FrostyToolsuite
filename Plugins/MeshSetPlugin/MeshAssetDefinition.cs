using FrostySdk.Attributes;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Windows;
using Frosty.Core;
using System.Windows.Media;
using Frosty.Core.Windows;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.Managers.Entries;
using MeshSetPlugin.Resources;
using MeshSetPlugin.Editors;

namespace MeshSetPlugin
{
    public enum MeshExportVersion
    {
        FBX_2012,
        FBX_2013,
        FBX_2014,
        FBX_2015,
        FBX_2016,
        FBX_2017
    }

    public enum MeshExportScale
    {
        Millimeters,
        Centimeters,
        Meters,
        Kilometers
    }

    public class MeshExportSettings
    {
        public MeshExportVersion Version { get; set; }

        public MeshExportScale Scale { get; set; }

        public bool FlattenHierarchy { get; set; }

        public bool ExportSingleLod { get; set; }

        public bool ExportAdditionalMeshes { get; set; }
    }

    public class SkinnedMeshExportSettings : MeshExportSettings
    {
        [DisplayName("Skeleton")]
        [Editor(typeof(FrostySkeletonEditor))]
        public string SkeletonAsset { get => skeletonAsset; set => skeletonAsset = value; }
        private string skeletonAsset = "";
    }

    public class MeshAssetDefinition : AssetDefinition
    {
        protected static ImageSource rigidMeshSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/MeshSetPlugin;component/Images/RigidMeshFileType.png") as ImageSource;
        protected static ImageSource skinnedMeshSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/MeshSetPlugin;component/Images/SkinnedMeshFileType.png") as ImageSource;
        protected static ImageSource compositeMeshSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/MeshSetPlugin;component/Images/CompositeMeshFileType.png") as ImageSource;

        public MeshAssetDefinition()
        {
        }

        public override void GetSupportedExportTypes(List<AssetExportType> exportTypes)
        {
            exportTypes.Add(new AssetExportType("fbx", "Autodesk FBX"));
            //exportTypes.Add(new AssetExportType("fbx", "Autodesk FBX (ASCII)"));
            exportTypes.Add(new AssetExportType("obj", "Wavefront OBJ"));

            base.GetSupportedExportTypes(exportTypes);
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyMeshSetEditor(logger);
        }

        public override bool Export(EbxAssetEntry entry, string path, string filterType)
        {
            if (!base.Export(entry, path, filterType))
            {
                if (filterType == "fbx" || filterType == "obj")
                {
                    if (PreExport(entry, out MeshExportSettings settings))
                    {
                        // export
                        ExportInternal(entry, path, settings, filterType);

                        // save out settings to config
                        PostExport(settings);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool PreExport(EbxAssetEntry entry, out MeshExportSettings outSettings)
        {
            MeshExportSettings settings = (entry.Type == "SkinnedMeshAsset")
                ? new SkinnedMeshExportSettings()
                : new MeshExportSettings();

            string Version = Config.Get<string>("MeshSetExportVersion", "FBX_2012", ConfigScope.Game);
            string Scale = Config.Get<string>("MeshSetExportScale", "Centimeters", ConfigScope.Game);
            bool flattenHierarchy = Config.Get<bool>("MeshSetExportFlattenHierarchy", false, ConfigScope.Game);
            bool exportSingleLod = Config.Get<bool>("MeshSetExportExportSingleLod", false, ConfigScope.Game);
            bool exportAdditionalMeshes = Config.Get<bool>("MeshSetExportExportAdditionalMeshes", false, ConfigScope.Game);
            string skeleton = Config.Get<string>("MeshSetExportSkeleton", "", ConfigScope.Game);

            settings.Version = (MeshExportVersion)Enum.Parse(typeof(MeshExportVersion), Version);
            settings.Scale = (MeshExportScale)Enum.Parse(typeof(MeshExportScale), Scale);
            settings.FlattenHierarchy = flattenHierarchy;
            settings.ExportSingleLod = exportSingleLod;
            settings.ExportAdditionalMeshes = exportAdditionalMeshes;

            if (settings is SkinnedMeshExportSettings exportSettings)
            {
                exportSettings.SkeletonAsset = skeleton;
            }

            outSettings = settings;
            return FrostyImportExportBox.Show<MeshExportSettings>("Mesh Export Settings", FrostyImportExportType.Export, settings) == MessageBoxResult.OK;
        }

        private void ExportInternal(EbxAssetEntry entry, string path, MeshExportSettings settings, string filterType)
        {
            // get ebx
            EbxAsset asset = App.AssetManager.GetEbx(entry);
            dynamic meshAsset = (dynamic)asset.RootObject;

            // get mesh res
            ulong resRid = meshAsset.MeshSetResource;
            ResAssetEntry rEntry = App.AssetManager.GetResEntry(resRid);
            MeshSet meshSet = App.AssetManager.GetResAs<MeshSet>(rEntry);

            // get skeleton (if required)
            string skeleton = "";
            if (meshSet.Type == MeshType.MeshType_Skinned)
            {
                skeleton = ((SkinnedMeshExportSettings)settings).SkeletonAsset;
            }

            FrostyTaskWindow.Show("Exporting MeshSet", "", (task) =>
            {
                FBXExporter exporter = new FBXExporter(task);
                exporter.ExportFBX(meshAsset, path, settings.Version.ToString().Replace("FBX_", ""), settings.Scale.ToString(), settings.FlattenHierarchy, settings.ExportSingleLod, skeleton, (filterType == "fbx") ? "binary" : "obj", meshSet);
            });
        }

        private void PostExport(MeshExportSettings settings)
        {
            Config.Add("MeshSetExportVersion", settings.Version.ToString(), ConfigScope.Game);
            Config.Add("MeshSetExportScale", settings.Scale.ToString(), ConfigScope.Game);
            Config.Add("MeshSetExportFlattenHierarchy", settings.FlattenHierarchy, ConfigScope.Game);
            Config.Add("MeshSetExportExportSingleLod", settings.ExportSingleLod, ConfigScope.Game);
            Config.Add("MeshSetExportExportAdditionalMeshes", settings.ExportAdditionalMeshes, ConfigScope.Game);

            if (settings is SkinnedMeshExportSettings exportSettings)
            {
                Config.Add("MeshSetExportSkeleton", exportSettings.SkeletonAsset, ConfigScope.Game);
            }

            Config.Save();
        }
    }

    public class RigidMeshAssetDefinition : MeshAssetDefinition
    {
        public override ImageSource GetIcon()
        {
            return rigidMeshSource;
        }
    }
    public class SkinnedMeshAssetDefinition : MeshAssetDefinition
    {
        public override ImageSource GetIcon()
        {
            return skinnedMeshSource;
        }
    }
    public class CompositeMeshAssetDefinition : MeshAssetDefinition
    {
        public override ImageSource GetIcon()
        {
            return compositeMeshSource;
        }
    }
}
