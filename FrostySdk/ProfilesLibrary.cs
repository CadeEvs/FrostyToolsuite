using System;
using System.Collections.Generic;
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
        StarWarsSquadrons = 20201001
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
        public static IProfile Profile => effectiveProfile.ProfileData;
        public static string ProfileName => effectiveProfile.Name;
        public static string DisplayName => effectiveProfile.DisplayName;
        public static string CacheName => effectiveProfile.CacheName;
        public static Type Deobfuscator => Type.GetType(DeobfuscatorNamespace + "." + effectiveProfile.Deobfuscator);
        public static Type AssetLoader => Type.GetType(AssetLoaderNamespace + "+" + effectiveProfile.AssetLoader);
        public static int DataVersion => effectiveProfile.DataVersion;
        public static List<FileSystemSource> Sources => effectiveProfile.Sources;
        public static string SDKFilename => effectiveProfile.SDKFilename;
        public static byte[] Banner => effectiveProfile.Banner;

        public static int EbxVersion => effectiveProfile.EbxVersion;
        public static bool RequiresKey => effectiveProfile.RequiresKey;
        public static bool MustAddChunks => effectiveProfile.MustAddChunks;
        public static bool EnableExecution => effectiveProfile.EnableExecution;
        public static bool ContainsEAC => effectiveProfile.ContainsEAC;

        public static string DefaultDiffuse => effectiveProfile.DefaultDiffuse;
        public static string DefaultNormals => effectiveProfile.DefaultNormals;
        public static string DefaultMask => effectiveProfile.DefaultMask;
        public static string DefaultTint => effectiveProfile.DefaultTint;

        public static bool HasLoadedProfile => effectiveProfile.ProfileData != null;

        public static Dictionary<int, string> SharedBundles => effectiveProfile.SharedBundles;

        public static bool IsResTypeIgnored(Managers.ResourceType resType)
        {
            return effectiveProfile.IgnoredResTypes.Contains((uint)resType);
        }

        private static Profile effectiveProfile;
        private static readonly string DeobfuscatorNamespace = typeof(Deobfuscators.NullDeobfuscator).Namespace;
        private static readonly string AssetLoaderNamespace = typeof(Managers.AssetManager).FullName;
        private static readonly byte[][] ObfuscationKey =
        {
            new byte[] { 0x46, 0x54, 0x76, 0x21, 0x37, 0x54 },
            new byte[] { 0x48, 0x52, 0x32, 0x45, 0x56, 0x29 },
            new byte[] { 0x4B, 0x5A, 0x4F, 0x52, 0x36, 0x2A },
            new byte[] { 0x4D, 0x56, 0x43, 0x53, 0x3A, 0x52 },
            new byte[] { 0x50, 0x5D, 0x46, 0x5A, 0x54, 0x2D },
            new byte[] { 0x56, 0x50, 0x4A, 0x25, 0x43, 0x59 },
        };

        private static List<Profile> profiles = new List<Profile>();

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

                    profiles.Add(profileStruct);
                }
            }

            // Add profiles from plugins
            foreach (Profile profile in pluginProfiles)
                profiles.Add(profile);
        }

        public static bool Initialize(string profileKey)
        {
            Profile? profile = profiles.Find((Profile a) => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase));
            if (!profile.HasValue)
                return false;
            effectiveProfile = profile.Value;

            return true;
        }

        public static bool HasProfile(string profileKey)
        {
            return profiles.FindIndex((Profile a) => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase)) != -1;
        }

        private static string DecodeString(NativeReader reader)
        {
            int length = reader.Read7BitEncodedInt();
            byte[] b = reader.ReadBytes(length);

            for (int i = 0; i < length; i++)
            {
                b[i] = (byte)(b[i] ^ ObfuscationKey[i % ObfuscationKey.Length][(i + ObfuscationKey.Length * (0x1000 | i)) % ObfuscationKey.Length]);
            }
            return Encoding.UTF8.GetString(b);
        }
    }
}
