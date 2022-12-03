using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FrostySdk;
using FrostySdk.Deobfuscators;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace FrostyCmd
{
    internal static class WriterExtensions
    {
        public static void WriteObfuscatedString(this NativeWriter writer, string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            byte[] key = new byte[] { 0x46, 0x52, 0x4F, 0x53, 0x54, 0x59 };

            writer.Write7BitEncodedInt(b.Length);
            for (int i = 0; i < b.Length; i++)
            {
                byte bv = (byte)(b[i] ^ key[i % key.Length]);
                writer.Write(bv);
            }
        }
    }

    public class ProfileFlags
    {
        byte MustAddChunks;
        byte EbxVersion;
        byte RequiresKey;
        byte ReadOnly;
        byte ContainsEAC;

        public ProfileFlags(byte mustAddChunks, byte ebxVersion, byte requiresKey, byte readOnly = 0, byte containsEAC = 0)
        {
            MustAddChunks = mustAddChunks;
            EbxVersion = ebxVersion;
            RequiresKey = requiresKey;
            ReadOnly = readOnly;
            ContainsEAC = containsEAC;
        }

        public void Write(NativeWriter writer)
        {
            writer.Write(MustAddChunks);
            writer.Write(EbxVersion);
            writer.Write(RequiresKey);
            writer.Write(ReadOnly);
            writer.Write(ContainsEAC);
        }
    }

    public class ProfileCreator
    {
        Dictionary<string, byte[]> blobs = new Dictionary<string, byte[]>();

        #region -- Helper Functions --

        private byte[] CreateSources(params string[] paths)
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(paths.Length);
                foreach (string path in paths)
                {
                    string[] arr = path.Split(';');
                    byte subPaths = (byte)(bool.Parse(arr[1]) ? 1 : 0);

                    writer.WriteObfuscatedString(arr[0]);
                    writer.Write(subPaths);
                }

                return writer.ToByteArray();
            }
        }

        static byte[] CreateBanner(string banner)
        {
            FileInfo fi = new FileInfo("Banners/" + banner + ".png");
            byte[] buffer = null;

            if (fi.Exists)
            {
                using (NativeReader reader = new NativeReader(new FileStream("Banners/" + banner + ".png", FileMode.Open, FileAccess.Read)))
                    buffer = reader.ReadToEnd();
            }
            else
            {
                buffer = new byte[] { };
            }

            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(buffer.Length);
                writer.Write(buffer);
                return writer.ToByteArray();
            }
        }

        #endregion

        #region -- Profiles --
        
        private void CreateBF4Profile()
        {
            string key = "bf4";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Battlefield™ 4");
                writer.Write((int)(int)ProfileVersion.Battlefield4);
                writer.WriteObfuscatedString("bf4");
                writer.WriteObfuscatedString(typeof(DADeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("LegacyAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("BF4SDK");
                writer.Write(CreateBanner("BF4"));
                writer.WriteObfuscatedString("EditorData/Grid_128_D");
                writer.WriteObfuscatedString("Architecture/_Textures/NormalmapDefault_N");
                writer.WriteObfuscatedString("EditorData/Grid_128_D");
                writer.WriteObfuscatedString("EditorData/Grid_128_D");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateBFHProfile()
        {
            string key = "bfh";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Battlefield™ Hardline");
                writer.Write((int)(int)ProfileVersion.Battlefield4);
                writer.WriteObfuscatedString("bfh");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("LegacyAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("BFHSDK");
                writer.Write(CreateBanner("BFH"));
                writer.WriteObfuscatedString("havana/Textures/_Defaults/Debug_D");
                writer.WriteObfuscatedString("Havana/Textures/_Defaults/Debug_N");
                writer.WriteObfuscatedString("havana/Textures/_Defaults/Debug_D");
                writer.WriteObfuscatedString("havana/Textures/_Defaults/Debug_D");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateNFS14Profile()
        {
            string key = "NFS14";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Need for Speed™ Rivals");
                writer.Write((int)(int)ProfileVersion.NeedForSpeedRivals);
                writer.WriteObfuscatedString("nfs14");
                writer.WriteObfuscatedString(typeof(DADeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("LegacyAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("NFS14SDK");
                writer.Write(CreateBanner("nfs14"));
                writer.WriteObfuscatedString("Shaders/Textures/defaultTextures/default_D");
                writer.WriteObfuscatedString("Shaders/Textures/defaultTextures/default_N");
                writer.WriteObfuscatedString("Shaders/Textures/defaultTextures/defaultBlack_D");
                writer.WriteObfuscatedString("Shaders/Textures/defaultTextures/defaultBlack_D");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateDAProfile()
        {
            string key = "DragonAgeInquisition";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Dragon Age™: Inquisition");
                writer.Write((int)(int)ProfileVersion.DragonAgeInquisition);
                writer.WriteObfuscatedString("dragonage");
                writer.WriteObfuscatedString(typeof(DADeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("LegacyAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("DragonAgeSDK");
                writer.Write(CreateBanner("DAI"));
                writer.WriteObfuscatedString("DA3/Actors/BaseMaterials/Patterns/Fabric/generic_cloth_swatch_d");
                writer.WriteObfuscatedString("DA3/shaders/Textures/NormalmapDefault_N");
                writer.WriteObfuscatedString("DA3/Actors/BaseMaterials/Patterns/Fabric/generic_cloth_swatch_s");
                writer.WriteObfuscatedString("DA3/Actors/BaseMaterials/Patterns/Fabric/generic_cloth_swatch_t");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(1, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateNFSProfile()
        {
            string key = "NFS16";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Need for Speed™");
                writer.Write((int)(int)ProfileVersion.NeedForSpeed);
                writer.WriteObfuscatedString("nfs16");
                writer.WriteObfuscatedString(typeof(DADeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("LegacyAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("NFS16SDK");
                writer.Write(CreateBanner("NFS"));
                writer.WriteObfuscatedString("Textures/T_white_A2");
                writer.WriteObfuscatedString("Textures/Generic/NormalmapDefault_N");
                writer.WriteObfuscatedString("Textures/T_white_A2");
                writer.WriteObfuscatedString("Textures/T_white_A2");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateSWProfile()
        {
            string key = "starwarsbattlefront";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("STAR WARS™ Battlefront™");
                writer.Write((int)(int)ProfileVersion.StarWarsBattlefront);
                writer.WriteObfuscatedString("starwars");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("StarWarsSDK");
                writer.Write(CreateBanner("SWBF"));
                writer.WriteObfuscatedString("shaders/T_DefaultOrange_C");
                writer.WriteObfuscatedString("shaders/T_DefaultNormal_NS");
                writer.WriteObfuscatedString("shaders/T_DefaultMask_MSW");
                writer.WriteObfuscatedString("shaders/T_DefaultBlack_C");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreatePVZ1Profile()
        {
            string key = "PVZ.Main_Win64_Retail";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Plants vs Zombies™ Garden Warfare");
                writer.Write((int)(int)ProfileVersion.PlantsVsZombiesGardenWarfare);
                writer.WriteObfuscatedString("pvz1");
                writer.WriteObfuscatedString(typeof(PVZDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("LegacyAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("PVZ1SDK");
                writer.Write(CreateBanner("PVZ"));
                writer.WriteObfuscatedString("_pvz/Shaders/_System/BaseShaders/default_textures/white");
                writer.WriteObfuscatedString("_pvz/Shaders/_System/BaseShaders/default_textures/normal_default");
                writer.WriteObfuscatedString("_pvz/Shaders/_System/BaseShaders/default_textures/white");
                writer.WriteObfuscatedString("_pvz/Shaders/_System/BaseShaders/default_textures/white");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreatePVZ2Profile()
        {
            string key = "GW2.Main_Win64_Retail";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Plants vs Zombies™ Garden Warfare 2");
                writer.Write((int)(int)ProfileVersion.PlantsVsZombiesGardenWarfare2);
                writer.WriteObfuscatedString("pvz2");
                writer.WriteObfuscatedString(typeof(PVZDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("LegacyAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("PVZ2SDK");
                writer.Write(CreateBanner("PVZ2"));
                writer.WriteObfuscatedString("art/Shaders/_System/BaseShaders/default_textures/white");
                writer.WriteObfuscatedString("art/Shaders/_System/BaseShaders/default_textures/normal_default");
                writer.WriteObfuscatedString("art/Shaders/_System/BaseShaders/default_textures/white");
                writer.WriteObfuscatedString("art/Shaders/_System/BaseShaders/default_textures/white");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateMECProfile()
        {
            string key = "MirrorsEdgeCatalyst";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Mirror's Edge™ Catalyst");
                writer.Write((int)(int)ProfileVersion.MirrorsEdgeCatalyst);
                writer.WriteObfuscatedString("mirrorsedge");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("MirrorsEdgeSDK");
                writer.Write(CreateBanner("MEC"));
                writer.WriteObfuscatedString("Objects/Props/SharedTexture/Generic/Default_Prop_Metal_D");
                writer.WriteObfuscatedString("Systems/Terrain/MeshScattering/NormalmapDefault_N");
                writer.WriteObfuscatedString("Objects/Props/SharedTexture/Generic/Default_Prop_Metal_RSM");
                writer.WriteObfuscatedString("Objects/Props/SharedTexture/Generic/Default_Prop_Metal_RSM");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateFIFA17Profile()
        {
            string key = "FIFA17";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("FIFA 17");
                writer.Write((int)(int)ProfileVersion.Fifa17);
                writer.WriteObfuscatedString("fifa17");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Update\\Patch\\Data;false", "Data;false"));
                writer.WriteObfuscatedString("Fifa17SDK");
                writer.Write(CreateBanner("FIFA17"));
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_normal");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateBF1Profile()
        {
            string key = "bf1";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Battlefield™ 1");
                writer.Write((int)(int)ProfileVersion.Battlefield1);
                writer.WriteObfuscatedString("bf1");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("BF1SDK");
                writer.Write(CreateBanner("BF1"));
                writer.WriteObfuscatedString("Shaders/Textures/T_DefaultGrey50_CS");
                writer.WriteObfuscatedString("Shaders/Textures/T_DefaultNormal_N");
                writer.WriteObfuscatedString("Shaders/Textures/T_DefaultGrey50_C");
                writer.WriteObfuscatedString("Shaders/Textures/T_DefaultGrey50_C");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateMEAProfile()
        {
            string key = "MassEffectAndromeda";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Mass Effect: Andromeda");
                writer.Write((int)(int)ProfileVersion.MassEffectAndromeda);
                writer.WriteObfuscatedString("masseffect");
                writer.WriteObfuscatedString(typeof(MEADeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("MassEffectSDK");
                writer.Write(CreateBanner("MEA"));
                writer.WriteObfuscatedString("Systems/Terrain/DefaultGrid");
                writer.WriteObfuscatedString("Textures/Generic/NormalmapDefault_N");
                writer.WriteObfuscatedString("Placeholder/Lookdev/MEC_Lookdev/textures/Default_Black");
                writer.WriteObfuscatedString("Placeholder/Lookdev/MEC_Lookdev/textures/Default_Black");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 2, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateFIFA18Profile()
        {
            string key = "FIFA18";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("FIFA 18");
                writer.Write((int)(int)ProfileVersion.Fifa18);
                writer.WriteObfuscatedString("fifa18");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("Fifa18SDK");
                writer.Write(CreateBanner("FIFA18"));
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_normal");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 4, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateNFS2017Profile()
        {
            string key = "NeedForSpeedPayback";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Need for Speed™ Payback");
                writer.Write(20171110);
                writer.WriteObfuscatedString("nfs17");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("NFS17SDK");
                writer.Write(CreateBanner("nfs17"));
                writer.WriteObfuscatedString("Characters/common/textures/debug/common_debug_texture_color");
                writer.WriteObfuscatedString("Textures/Generic/defaultNormalMap_N");
                writer.WriteObfuscatedString("Characters/common/textures/debug/common_debug_texture_coeff");
                writer.WriteObfuscatedString("Characters/common/textures/debug/common_debug_texture_coeff");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 4, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateSW2Profile()
        {
            string key = "starwarsbattlefrontii";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("STAR WARS™ Battlefront™ II");
                writer.Write((int)(int)ProfileVersion.StarWarsBattlefrontII);
                writer.WriteObfuscatedString("starwarsii");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("ManifestAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("StarWarsIISDK");
                writer.Write(CreateBanner("SWBF2"));
                writer.WriteObfuscatedString("shaders/T_DefaultWhite_C");
                writer.WriteObfuscatedString("shaders/T_DefaultNormal_NWM");
                //writer.WriteObfuscatedString("shaders/T_DefaultGrey22_C");
                writer.WriteObfuscatedString("shaders/T_DefaultBlack_C");
                writer.WriteObfuscatedString("shaders/T_DefaultBlack_C");

                string[] sharedBundleNames = new string[]
                {
                    "win32/gameplay/bundles/sharedbundles/frontend+mp/abilities/sharedbundleabilities_frontend+mp",
                    "win32/gameplay/bundles/sharedbundles/common/animation/sharedbundleanimation_common",
                    "win32/gameplay/bundles/sharedbundles/frontend+mp/characters/sharedbundlecharacters_frontend+mp",
                    "win32/gameplay/bundles/sharedbundles/common/vehicles/sharedbundlevehiclescockpits",
                    "win32/gameplay/bundles/sharedbundles/common/characters/sharedbundlecharacters1p",
                    "win32/ui/frontend/webbrowser/webbrowserresourcebundle",
                    "win32/systems/frostbitestartupdata",
                    "win32/gameplay/wrgameconfiguration",
                    "win32/default_settings",
                    "win32/s1/gameplay/bundles/sharedbundleseason1",
                    "win32/gameplay/bundles/sp/vehicle/sharedbundle_sp_vehicle",
                    "win32/gameplay/bundles/sp/sharedbundle_sp",
                    "win32/gameplay/bundles/sp/player/sharedbundle_sp_player",
                    "win32/gameplay/bundles/sp/droid/sharedbundle_sp_droid",
                    "win32/gameplay/bundles/sp/buddy/sharedbundle_sp_buddy",
                    "win32/gameplay/bundles/sharedbundles/sp/vehicles/sharedbundlevehicles_sp",
                    "win32/gameplay/bundles/sharedbundles/sp/abilities/sharedbundleabilities_sp",
                    "win32/a3/gameplay/bundles/sp/vehicle/sharedbundle_sp_vehicle_a3",
                    "win32/a3/gameplay/bundles/sp/sharedbundle_sp_a3",
                    "win32/a3/gameplay/bundles/sp/player/sharedbundle_sp_player_a3",
                    "win32/a3/gameplay/bundles/sp/buddy/sharedbundle_sp_buddy_a3",
                    "win32/ui/static",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_worstcase",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_traditionalchinese",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_spanishmex",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_spanish",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_simplifiedchinese",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_russian",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_polish",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_korean",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_japanese",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_italian",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_german",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_french",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_english",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_brazilianportuguese",
                    "win32/ui/resources/fonts/wsuiimfontconfiguration_languageformat_arabicsa",
                    "win32/sound/sp/music/screens/sw02_sp_music_loading_bundleasset:8f175ba0-cac3-44bd-87d1-710706c09278",
                    "win32/sound/music/loading/sw02_music_loading_bundleasset_initialexperience:c2d5acdb-7a2f-4742-9499-2cd74fefec4c",
                    "win32/sound/music/loading/sw02_music_loading_bundleasset:db988158-79d3-489b-96d8-970deca60c67",
                    "win32/loadingscreens_bundle",
                    "win32/gameplay/bundles/sharedbundles/common/weapons/sharedbundleweapons_common",
                    "win32/persistence/wsmppersistence",
                };

                writer.Write(sharedBundleNames.Length);
                foreach (string sbn in sharedBundleNames)
                    writer.WriteObfuscatedString(sbn);
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 4, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateMadden19Profile()
        {
            string key = "Madden19";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Madden NFL 19™");
                writer.Write((int)(int)ProfileVersion.Madden19);
                writer.WriteObfuscatedString("madden19");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("StandardAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("MADDEN19SDK");
                writer.Write(CreateBanner("madden19"));
                writer.WriteObfuscatedString("Longshot/Common/Debug_Grey");
                writer.WriteObfuscatedString("Longshot/Common/Debug_Normal");
                writer.WriteObfuscatedString("Longshot/Common/Default_S");
                writer.WriteObfuscatedString("Longshot/Common/Debug_Grey");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 4, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateFIFA19Profile()
        {
            string key = "FIFA19";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("FIFA 19");
                writer.Write((int)(int)ProfileVersion.Fifa19);
                writer.WriteObfuscatedString("fifa19");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("FifaAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("FIFA19SDK");
                writer.Write(CreateBanner("fifa19"));
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_normal");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 4, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateBFVProfile()
        {
            string key = "bfv";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Battlefield™ V");
                writer.Write((int)(int)ProfileVersion.Battlefield5);
                writer.WriteObfuscatedString("bfv");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("ManifestAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("BFVSDK");
                writer.Write(CreateBanner("bfv"));
                writer.WriteObfuscatedString("Shaders/Systems/Debug_D");
                writer.WriteObfuscatedString("Shaders/Systems/Debug_N");
                writer.WriteObfuscatedString("Shaders/Systems/Debug_SRM");
                writer.WriteObfuscatedString("Shaders/Systems/Debug_D");

                string[] sharedBundleNames = new string[]
                {
                    "win32/gameplay/blueprintbundles/vehiclescommonbundle",
                    "win32/gameplay/blueprintbundles/charactercommonmpbundle",
                    "win32/gameplay/weapons/bundling/weaponsbundlecommon",
                    "win32/animations/commonanimationbundle",
                    "win32/ui/static",
                    "win32/default_settings",
                    "win32/gameconfigurations/warsaw",
                    "win32/systems/frostbitestartupdata",
                    "win32/loadingscreens_bundle",
                    "win32/webbrowser/webbrowserbundle",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_simplifiedchinese",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_worstcase",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_arabicsa",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_brazilianportuguese",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_korean",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_traditionalchinese",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_polish",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_russian",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_japanese",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_italian",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_spanishMex",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_spanish",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_german",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_french",
                    "win32/ui/Fonts/fontassets/fontconfiguration_languageformat_english",
                    "win32/sound/music/loadingmusicplayer:5c5a6f4f-8d55-a023-fbef-f4bf6c067209",
                    "win32/twinkle/ui/twinklevideos",
                    "win32/sparta/offline/spartabundles"
                };

                writer.Write(sharedBundleNames.Length);
                foreach (string sbn in sharedBundleNames)
                    writer.WriteObfuscatedString(sbn);

                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 4, 0, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateBFVOBProfile()
        {
            string key = "bfvob";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Battlefield™ V");
                writer.Write((int)(int)ProfileVersion.Battlefield5);
                writer.WriteObfuscatedString("bfv");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("ManifestAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("BFVSDK");
                writer.Write(CreateBanner("bfV"));
                writer.WriteObfuscatedString("Systems/Textures/Debug_D");
                writer.WriteObfuscatedString("Systems/Textures/Debug_N");
                writer.WriteObfuscatedString("Systems/Textures/Debug_SRM");
                writer.WriteObfuscatedString("Systems/Textures/Debug_D");

                string[] sharedBundleNames = new string[]
                {
                };

                writer.Write(sharedBundleNames.Length);
                foreach (string sbn in sharedBundleNames)
                    writer.WriteObfuscatedString(sbn);

                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 4, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateNFSEdgeProfile()
        {
            string key = "NFSOL";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Need for Speed™ ONLINE");
                writer.Write((int)(int)ProfileVersion.NeedForSpeedEdge);
                writer.WriteObfuscatedString("nfsedge");
                writer.WriteObfuscatedString(typeof(DADeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("EdgeAssetLoader"));
                writer.Write(CreateSources("Data;false"));
                writer.WriteObfuscatedString("NFSEDGESDK");
                writer.Write(CreateBanner("nfsedge"));
                writer.WriteObfuscatedString("Shaders/Textures/debugTextures/debugGrid_D");
                writer.WriteObfuscatedString("Shaders/Textures/defaultTextures/default_N");
                writer.WriteObfuscatedString("Shaders/Textures/debugTextures/debugGrid_D");
                writer.WriteObfuscatedString("Shaders/Textures/debugTextures/debugGrid_D");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 2, 0, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateAnthemProfile()
        {
            string key = "Anthem";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Anthem™");
                writer.Write((int)(int)ProfileVersion.Anthem);
                writer.WriteObfuscatedString("anthem");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("AnthemSDK");
                writer.Write(CreateBanner("anthem"));
                writer.WriteObfuscatedString("Shaders/Utils/NullTextures/Black_Null");
                writer.WriteObfuscatedString("Shaders/Utils/NullTextures/Normal_Null");
                writer.WriteObfuscatedString("Shaders/Utils/NullTextures/Black_Null");
                writer.WriteObfuscatedString("Shaders/Utils/NullTextures/Black_Null");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 5, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateMadden20Profile()
        {
            string key = "Madden20";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Madden NFL 20™");
                writer.Write((int)(int)ProfileVersion.Madden20);
                writer.WriteObfuscatedString("madden20");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("FifaAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("MADDEN20SDK");
                writer.Write(CreateBanner("madden20"));
                writer.WriteObfuscatedString("Longshot/Common/Debug_Grey");
                writer.WriteObfuscatedString("content/common/textures/debug/debug_texture_norm");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("Longshot/Common/Debug_Grey");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 4, 0);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreatePVZ3Profile()
        {
            string key = "PVZBattleforNeighborville";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Plants vs Zombies: Battle for Neighborville™");
                writer.Write((int)(int)ProfileVersion.PlantsVsZombiesBattleforNeighborville);
                writer.WriteObfuscatedString("PVZ3");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("PVZ3SDK");
                writer.Write(CreateBanner("PVZ3"));
                writer.WriteObfuscatedString("GW2_art/Shaders/_System/BaseShaders/default_textures/white");
                writer.WriteObfuscatedString("GW2_art/Shaders/_System/BaseShaders/default_textures/normal_default");
                writer.WriteObfuscatedString("GW2_art/Shaders/_System/BaseShaders/default_textures/white");
                writer.WriteObfuscatedString("GW2_art/Shaders/_System/BaseShaders/default_textures/white");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 5, 1, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateFifa20Profile()
        {
            string key = "FIFA20";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("FIFA 20");
                writer.Write((int)(int)ProfileVersion.Fifa20);
                writer.WriteObfuscatedString("fifa20");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("FifaAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("FIFA20SDK");
                writer.Write(CreateBanner("fifa20"));
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_normal");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey)
                ProfileFlags pf = new ProfileFlags(0, 5, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateNFSHeatProfile()
        {
            string key = "NeedForSpeedHeat";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Need for Speed™ Heat");
                writer.Write((int)(int)ProfileVersion.NeedForSpeedHeat);
                writer.WriteObfuscatedString("NFSHEAT");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("NFSHEATSDK");
                writer.Write(CreateBanner("NFSHEAT"));
                writer.WriteObfuscatedString("Shaders/GenericTextures/UT_default_D");
                writer.WriteObfuscatedString("Shaders/GenericTextures/UT_default_N");
                writer.WriteObfuscatedString("Shaders/GenericTextures/UT_defaultBlack_D");
                writer.WriteObfuscatedString("Shaders/GenericTextures/UT_default_D");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly?)
#if FROSTY_DEVELOPER || FROSTY_ALPHA
                ProfileFlags pf = new ProfileFlags(0, 4, 1);
#else
                ProfileFlags pf = new ProfileFlags(0, 4, 1, 1);
#endif
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateSWSProfile()
        {
            string key = "starwarssquadrons";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("STAR WARS™: Squadrons");
                writer.Write((int)(int)ProfileVersion.StarWarsSquadrons);
                writer.WriteObfuscatedString("starwarsiii");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("ManifestAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Update;true", "Data;false"));
                writer.WriteObfuscatedString("SWSSDK");
                writer.Write(CreateBanner("SWS"));
                writer.WriteObfuscatedString("Game/Shaders/Textures/T_DefaultWhite_C");
                writer.WriteObfuscatedString("Game/Shaders/Textures/T_DefaultNormal_N");
                writer.WriteObfuscatedString("Game/Shaders/Textures/T_Default_Midgray");
                writer.WriteObfuscatedString("Game/Shaders/Textures/T_Default_Midgray");

                string[] sharedBundleNames = new string[]
                {
                    "win32/game/ui/systems/uiassetsbundle_stategroups",
                    "win32/game/gameplay/starships/starships_sharedbundle",
                    "win32/game/levels/lobby/bundles/sharedbundlelobbylooseassets",
                    "win32/game/gameplay/bundles/sharedbundles/starships/sharedbundlestarshipscine",
                    "win32/game/gameplay/bundles/sharedbundles/pilots/sharedbundlepilots",
                    "win32/default_settings",
                    "win32/ui/static",
                    "win32/systems/frostbitestartupdata",
                    "win32/loadingscreens_bundle",
                    "win32/game/incomgameconfiguration",
                    "win32/game/gameplay/bundles/sharedbundles/starships/sharedbundlestarshipsgarage",
                    "win32/game/gameplay/bundles/sharedbundles/flairs/sharedbundleflairs",
                    "win32/game/gameplay/starships/starships_sp_sharedbundle",
                    "win32/sound/music/mav01_loadingmusic_title_and_frontend:89dce4fb-ac26-4be0-9f57-8969eaff7f28",
                    "win32/game/ui/frontend/webbrowser/webbrowserresourcebundle",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_english",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_french",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_german",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_spanish",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_spanishmex",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_italian",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_japanese",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_russian",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_polish",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_traditionalchinese",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_korean",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_brazilianportuguese",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_simplifiedchinese",
                    "win32/game/ui/assets/fonts/wsuiimfontconfiguration_languageformat_worstcase",
                };

                writer.Write(sharedBundleNames.Length);
                foreach (string sbn in sharedBundleNames)
                    writer.WriteObfuscatedString(sbn);

                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly, EAC)
                ProfileFlags pf = new ProfileFlags(0, 4, 0, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateFifa21Profile()
        {
            string key = "FIFA21";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("FIFA 21");
                writer.Write((int)(int)ProfileVersion.Fifa21);
                writer.WriteObfuscatedString("fifa21");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("FIFA21SDK");
                writer.Write(CreateBanner("fifa21"));
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_normal");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 5, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateMadden22Profile()
        {
            string key = "Madden22";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Madden NFL 22™");
                writer.Write((int)(int)ProfileVersion.Madden22);
                writer.WriteObfuscatedString("madden22");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("MADDEN22SDK");
                writer.Write(CreateBanner("madden22"));
                writer.WriteObfuscatedString("content/common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/common/textures/debug/debug_texture_norm");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 6, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateFifa22Profile()
        {
            string key = "FIFA22";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("FIFA 22");
                writer.Write((int)(int)ProfileVersion.Fifa22);
                writer.WriteObfuscatedString("fifa22");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("FIFA22SDK");
                writer.Write(CreateBanner("fifa22"));
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_normal");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 6, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateBF2042Profile()
        {
            string key = "BF2042";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Battlefield™ 2042");
                writer.Write((int)(int)ProfileVersion.Battlefield2042);
                writer.WriteObfuscatedString("bf2042");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("BF2042SDK");
                writer.Write(CreateBanner("bf2042"));
                writer.WriteObfuscatedString("Common/Shaders/Textures/Debug/Debug_D");
                writer.WriteObfuscatedString("Common/Shaders/Textures/Debug/Debug_N");
                writer.WriteObfuscatedString("Common/Shaders/Textures/Debug/Debug_R");
                writer.WriteObfuscatedString("Common/Shaders/Textures/Debug/Debug_SRM");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly, EAC)
                ProfileFlags pf = new ProfileFlags(0, 6, 1, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateMadden23Profile()
        {
            string key = "Madden23";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Madden NFL 23™");
                writer.Write((int)(int)ProfileVersion.Madden23);
                writer.WriteObfuscatedString("madden23");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("MADDEN23SDK");
                writer.Write(CreateBanner("madden23"));
                writer.WriteObfuscatedString("content/common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/common/textures/debug/debug_texture_norm");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 6, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateFifa23Profile()
        {
            string key = "FIFA23";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("FIFA 23");
                writer.Write((int)(int)ProfileVersion.Fifa22);
                writer.WriteObfuscatedString("fifa23");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("FIFA23SDK");
                writer.Write(CreateBanner("fifa23"));
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_color");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_normal");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_coeff");
                writer.WriteObfuscatedString("content/Common/textures/debug/debug_texture_alpha");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 6, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }

        private void CreateNFSUnboundProfile()
        {
            string key = "NeedForSpeedUnbound";
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.WriteObfuscatedString("Need for Speed™ Unbound");
                writer.Write((int)(int)ProfileVersion.NeedForSpeedUnbound);
                writer.WriteObfuscatedString("nfsunbound");
                writer.WriteObfuscatedString(typeof(NullDeobfuscator).Name);
                writer.WriteObfuscatedString(AssetManager.GetLoaderName("CasAssetLoader"));
                writer.Write(CreateSources("Patch;false", "Data;false"));
                writer.WriteObfuscatedString("NFSUnboundSDK");
                writer.Write(CreateBanner("nfsunbound"));
                writer.WriteObfuscatedString("shaders/generictextures/ut_defaultwhite_d");
                writer.WriteObfuscatedString("shaders/generictextures/UT_Default_N");
                writer.WriteObfuscatedString("Shaders/GenericTextures/UT_defaultBlack_D");
                writer.WriteObfuscatedString("shaders/generictextures/ut_defaultwhite_d");
                writer.Write(0); // shared bundle names
                writer.Write(0); // ignored res types

                // Flags (MustAddChunks, EbxVersion, RequiresKey, ReadOnly)
                ProfileFlags pf = new ProfileFlags(0, 6, 1, 1);
                pf.Write(writer);

                blobs.Add(key, writer.ToByteArray());
            }
        }
        #endregion

        public ProfileCreator()
        {
        }

        public void CreateProfiles()
        {
            CreateDAProfile();
            CreateNFSProfile();
            CreateSWProfile();
            CreateMECProfile();
            CreateFIFA17Profile();
            CreateMEAProfile();
            CreateFIFA18Profile();
            CreateSW2Profile();
            CreateNFS2017Profile();
            CreateMadden19Profile();
            CreateBF1Profile();
            CreatePVZ1Profile();
            CreatePVZ2Profile();
            CreateFIFA19Profile();
            CreateBF4Profile();
            CreateNFS14Profile();
            CreateBFVProfile();
            CreateAnthemProfile();
            CreateMadden20Profile();
            CreateFifa20Profile();
            CreatePVZ3Profile();
            CreateNFSHeatProfile();
            CreateBFHProfile();
            CreateSWSProfile();

#if FROSTY_DEVELOPER

            CreateNFSEdgeProfile();
            CreateFifa21Profile();
            CreateFifa22Profile();
            CreateBF2042Profile();
            CreateMadden22Profile();
            CreateMadden23Profile();
            CreateNFSUnboundProfile();

#endif

            using (NativeWriter writer = new NativeWriter(new FileStream(@"..\..\..\..\FrostySdk\Profiles.bin", FileMode.Create)))
            {
                writer.Write(blobs.Count);

                long offset = 0;
                foreach (string key in blobs.Keys)
                {
                    writer.WriteObfuscatedString(key);
                    writer.Write(offset);
                    offset += blobs[key].Length;
                }

                foreach (byte[] blob in blobs.Values)
                    writer.Write(blob);
            }
        }
    }
}
