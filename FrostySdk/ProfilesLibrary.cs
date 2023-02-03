using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrostySdk.IO;
using System.Reflection;
using Frosty.Hash;
using FrostySdk.Interfaces;

namespace FrostySdk
{
    public struct FileSystemSource
    {
        public string Path;
        public bool SubDirs;
    }

    public enum ProfileVersion
    {
        NeedForSpeedRivals = 20131115,
        Battlefield4 = 20141117,
        DragonAgeInquisition = 20141118,
        NeedForSpeed = 20151103,
        StarWarsBattlefront = 20151117,
        PlantsVsZombiesGardenWarfare = 20140225,
        PlantsVsZombiesGardenWarfare2 = 20150223,
        MirrorsEdgeCatalyst = 20160607,
        Fifa17 = 20160927,
        Battlefield1 = 20161021,
        MassEffectAndromeda = 20170321,
        Fifa18 = 20170929,
        NeedForSpeedPayback = 20171110,
        StarWarsBattlefrontII = 20171117,
        Madden19 = 20180807,
        Fifa19 = 20180914,
        Battlefield5 = 20180628,
        NeedForSpeedEdge = 20171210,
        Anthem = 20181207,
        Madden20 = 20190729,
        PlantsVsZombiesBattleforNeighborville = 20190905,
        Fifa20 = 20190911,
        NeedForSpeedHeat = 20191101,
        StarWarsSquadrons = 20201001,
        Madden21 = 20200828,
        Fifa21 = 20201009,
        Madden22 = 20210820,
        Fifa22 = 20210927,
        Battlefield2042 = 20211119,
        Madden23 = 20220819,
        Fifa23 = 20220930,
        NeedForSpeedUnbound = 20221129,
        DeadSpace = 20230127
    }

    public struct Profile
    {
        public string Name;
        public string DisplayName;
        public IProfile ProfileData;
        public int DataVersion;
        public string CacheName;
        public string Deobfuscator;
        public string AssetLoader;
        public List<FileSystemSource> Sources;
        public string SDKFilename;
        public byte[] Banner;

        public int EbxVersion;
        public bool RequiresKey;
        public bool MustAddChunks;
        public bool EnableExecution;
        public bool ContainsEAC;

        public string DefaultDiffuse;
        public string DefaultNormals;
        public string DefaultMask;
        public string DefaultTint;

        public Dictionary<int, string> SharedBundles;
        public List<uint> IgnoredResTypes;
    }

    public static class ProfilesLibrary
    {
        public static IProfile Profile => m_effectiveProfile.ProfileData;
        public static string ProfileName => m_effectiveProfile.Name;
        public static string DisplayName => m_effectiveProfile.DisplayName;
        public static string CacheName => m_effectiveProfile.CacheName;
        public static Type Deobfuscator => Type.GetType(m_deobfuscatorNamespace + "." + m_effectiveProfile.Deobfuscator);
        public static Type AssetLoader => Type.GetType(m_assetLoaderNamespace + "+" + m_effectiveProfile.AssetLoader);
        public static int DataVersion => m_effectiveProfile.DataVersion;
        public static List<FileSystemSource> Sources => m_effectiveProfile.Sources;
        public static string SDKFilename => m_effectiveProfile.SDKFilename;
        public static byte[] Banner => m_effectiveProfile.Banner;

        public static int EbxVersion => m_effectiveProfile.EbxVersion;
        public static bool RequiresKey => m_effectiveProfile.RequiresKey;
        public static bool MustAddChunks => m_effectiveProfile.MustAddChunks;
        public static bool EnableExecution => m_effectiveProfile.EnableExecution;
        public static bool ContainsEAC => m_effectiveProfile.ContainsEAC;

        public static string DefaultDiffuse => m_effectiveProfile.DefaultDiffuse;
        public static string DefaultNormals => m_effectiveProfile.DefaultNormals;
        public static string DefaultMask => m_effectiveProfile.DefaultMask;
        public static string DefaultTint => m_effectiveProfile.DefaultTint;

        public static bool HasLoadedProfile => m_effectiveProfile.ProfileData != null;

        public static Dictionary<int, string> SharedBundles => m_effectiveProfile.SharedBundles;

        public static bool IsResTypeIgnored(Managers.Entries.ResourceType resType)
        {
            return m_effectiveProfile.IgnoredResTypes.Contains((uint)resType);
        }

        private static Profile m_effectiveProfile;
        private static readonly string m_deobfuscatorNamespace = typeof(Deobfuscators.NullDeobfuscator).Namespace;
        private static readonly string m_assetLoaderNamespace = typeof(Managers.AssetManager).FullName;
        private static readonly byte[][] m_obfuscationKey =
        {
            new byte[] { 0x46, 0x54, 0x76, 0x21, 0x37, 0x54 },
            new byte[] { 0x48, 0x52, 0x32, 0x45, 0x56, 0x29 },
            new byte[] { 0x4B, 0x5A, 0x4F, 0x52, 0x36, 0x2A },
            new byte[] { 0x4D, 0x56, 0x43, 0x53, 0x3A, 0x52 },
            new byte[] { 0x50, 0x5D, 0x46, 0x5A, 0x54, 0x2D },
            new byte[] { 0x56, 0x50, 0x4A, 0x25, 0x43, 0x59 },
        };

        private static readonly List<Profile> m_profiles = new List<Profile>();

        public static void Initialize(IEnumerable<Profile> pluginProfiles)
        {
            List<string> keys = new List<string>();
            List<long> offsets = new List<long>();

            using (NativeReader reader = new NativeReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("FrostySdk.Profiles.bin")))
            {
                int numProfiles = reader.ReadInt();
                for (int i = 0; i < numProfiles; i++)
                {
                    string key = DecodeString(reader);
                    long offset = reader.ReadLong();

                    keys.Add(key);
                    offsets.Add(offset);
                }

                long startPos = reader.Position;
                for (int i = 0; i < keys.Count; i++)
                {
                    reader.Position = startPos + offsets[i];
                    Profile profileStruct = new Profile();
                    {
                        profileStruct.Name = keys[i];
                        profileStruct.DisplayName = DecodeString(reader);
                        profileStruct.DataVersion = reader.ReadInt();
                        profileStruct.CacheName = DecodeString(reader);
                        profileStruct.Deobfuscator = DecodeString(reader);
                        profileStruct.AssetLoader = DecodeString(reader);
                        profileStruct.Sources = new List<FileSystemSource>();
                        profileStruct.SharedBundles = new Dictionary<int, string>();
                        profileStruct.IgnoredResTypes = new List<uint>();

                        int numSources = reader.ReadInt();
                        for (int j = 0; j < numSources; j++)
                        {
                            FileSystemSource source = new FileSystemSource();
                            source.Path = DecodeString(reader);
                            source.SubDirs = (reader.ReadByte() == 1);
                            profileStruct.Sources.Add(source);
                        }

                        profileStruct.SDKFilename = DecodeString(reader);
                        profileStruct.Banner = reader.ReadBytes(reader.ReadInt());
                        profileStruct.DefaultDiffuse = DecodeString(reader);
                        profileStruct.DefaultNormals = DecodeString(reader);
                        profileStruct.DefaultMask = DecodeString(reader);
                        profileStruct.DefaultTint = DecodeString(reader);

                        int numSharedBundles = reader.ReadInt();
                        for (int j = 0; j < numSharedBundles; j++)
                        {
                            string sharedBundle = DecodeString(reader);
                            profileStruct.SharedBundles.Add(Fnv1.HashString(sharedBundle.ToLower()), sharedBundle);
                        }

                        int numIgnoredResTypes = reader.ReadInt();
                        for (int j = 0; j < numIgnoredResTypes; j++)
                            profileStruct.IgnoredResTypes.Add(reader.ReadUInt());

                        profileStruct.MustAddChunks = (reader.ReadByte() == 1);
                        profileStruct.EbxVersion = reader.ReadByte();
                        profileStruct.RequiresKey = (reader.ReadByte() == 1);
                        profileStruct.EnableExecution = (reader.ReadByte() != 1);
                        profileStruct.ContainsEAC = reader.ReadByte() == 1;

                        profileStruct.ProfileData = new BaseFrostyProfile();
                    }

                    m_profiles.Add(profileStruct);
                }
            }

            // Add profiles from plugins
            foreach (Profile profile in pluginProfiles)
                m_profiles.Add(profile);
        }

        public static bool Initialize(string profileKey)
        {
            Profile? profile = m_profiles.Find((Profile a) => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase));
            m_effectiveProfile = profile.Value;

            return true;
        }

        public static bool HasProfile(string profileKey)
        {
            return m_profiles.FindIndex((Profile a) => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase)) != -1;
        }

        public static bool IsLoaded(params ProfileVersion[] versions)
        {
            return versions.Contains((ProfileVersion)DataVersion);
        }

        private static string DecodeString(NativeReader reader)
        {
            int length = reader.Read7BitEncodedInt();
            byte[] b = reader.ReadBytes(length);

            for (int i = 0; i < length; i++)
            {
                b[i] = (byte)(b[i] ^ m_obfuscationKey[i % m_obfuscationKey.Length][(i + m_obfuscationKey.Length * (0x1000 | i)) % m_obfuscationKey.Length]);
            }
            return Encoding.UTF8.GetString(b);
        }
    }
}
