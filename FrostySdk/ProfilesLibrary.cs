using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FrostySdk
{
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

        /// <summary>
        /// Determines if a collection of <see cref="ProfileVersion"/> enumerations contains the <see cref="ProfileVersion"/> of the loaded profile.
        /// </summary>
        /// <param name="versions">The collection of <see cref="ProfileVersion"/> enumerations to be checked, passed as a list of parameters.</param>
        /// <returns>A bool determining whether or not one of the specified profiles is loaded.</returns>
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
                b[i] = (byte)(b[i] ^ ObfuscationKey[i % ObfuscationKey.Length][(i + ObfuscationKey.Length * (0x1000 | i)) % ObfuscationKey.Length]);
            }
            return Encoding.UTF8.GetString(b);
        }
    }
}
