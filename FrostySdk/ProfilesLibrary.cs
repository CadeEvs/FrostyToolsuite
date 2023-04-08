using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Frosty.Sdk.Profiles;
using static Frosty.Sdk.Utils.Utils;

namespace Frosty.Sdk;

public static class ProfilesLibrary
{
    public static string ProfileName => s_effectiveProfile?.Name ?? string.Empty;
    public static string DisplayName => s_effectiveProfile?.DisplayName ?? string.Empty;
    public static string CacheName => s_effectiveProfile?.CacheName?? string.Empty;
    public static string Deobfuscator => s_effectiveProfile?.Deobfuscator ?? string.Empty;
    public static string AssetLoader => s_effectiveProfile?.AssetLoader ?? string.Empty;
    public static int DataVersion => s_effectiveProfile?.DataVersion ?? -1;
    public static List<FileSystemSource> Sources => s_effectiveProfile?.Sources ?? new List<FileSystemSource>();
    public static string SdkFilename => s_effectiveProfile?.SdkFileName ?? string.Empty;

    public static int EbxVersion => s_effectiveProfile?.EbxVersion ?? -1;
    public static bool RequiresKey => s_effectiveProfile?.RequiresKey ?? false;
    public static bool MustAddChunks => s_effectiveProfile?.MustAddChunks ?? false;
    public static bool EnableExecution => s_effectiveProfile?.EnableExecution ?? false;
    public static bool HasAntiCheat => s_effectiveProfile?.HasAntiCheat ?? false;
    
    public static CompressionType EbxCompression => (CompressionType)(s_effectiveProfile?.EbxCompression ?? 0);
    public static CompressionType ResCompression => (CompressionType)(s_effectiveProfile?.ResCompression ?? 0);
    public static CompressionType ChunkCompression => (CompressionType)(s_effectiveProfile?.ChunkCompression ?? 0);
    public static CompressionType TextureChunkCompression => (CompressionType)(s_effectiveProfile?.TextureChunkCompression ?? 0);
    public static int MaxBufferSize => s_effectiveProfile?.MaxBufferSize ?? 0;
    public static int ZStdCompressionLevel => s_effectiveProfile?.ZStdCompressionLevel ?? 0;

    public static string DefaultDiffuse => s_effectiveProfile?.DefaultDiffuse ?? string.Empty;
    public static string DefaultNormals => s_effectiveProfile?.DefaultNormals ?? string.Empty;
    public static string DefaultMask => s_effectiveProfile?.DefaultMask ?? string.Empty;
    public static string DefaultTint => s_effectiveProfile?.DefaultTint ?? string.Empty;

    public static bool HasLoadedProfile => s_effectiveProfile != null;

    public static readonly Dictionary<int, string> SharedBundles = new();

    private static Profile? s_effectiveProfile;
    private static bool s_initialized;
    private static readonly List<Profile> s_profiles = new();
    
    public static void Initialize()
    {
        foreach (string file in Directory.EnumerateFiles("Profiles"))
        {
            Profile? profile = JsonSerializer.Deserialize<Profile>(new FileStream(file, FileMode.Open, FileAccess.Read));
            if (profile != null)
            {
                s_profiles.Add(profile);
            }
        }

        s_initialized = true;
    }

    public static bool Initialize(string profileKey)
    {
        if (!s_initialized)
        {
            Initialize();
        }
        s_effectiveProfile = s_profiles.Find(a => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase));
        if (s_effectiveProfile != null)
        {
            foreach (string bundle in  s_effectiveProfile.SharedBundles)
            {
                SharedBundles.Add(HashString(bundle), bundle);
            }

            return true;
        }

        return false;
    }

    public static bool HasProfile(string profileKey)
    {
        return s_profiles.FindIndex(a => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase)) != -1;
    }

    public static bool IsLoaded(params ProfileVersion[] versions)
    {
        return versions.Contains((ProfileVersion)DataVersion);
    }
}