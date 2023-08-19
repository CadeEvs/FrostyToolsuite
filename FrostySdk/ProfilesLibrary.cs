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
    public static bool IsInitialized { get; private set; }
    
    public static string ProfileName => s_effectiveProfile?.Name ?? string.Empty;
    public static string DisplayName => s_effectiveProfile?.DisplayName ?? string.Empty;
    public static string InternalName => s_effectiveProfile?.InternalName?? string.Empty;
    public static string TypeInfoSignature => s_effectiveProfile?.TypeInfoSignature ?? string.Empty;
    public static bool HasStrippedTypeNames => s_effectiveProfile?.HasStrippedTypeNames ?? false;
    public static int DataVersion => s_effectiveProfile?.DataVersion ?? -1;
    public static FrostbiteVersion FrostbiteVersion => s_effectiveProfile?.FrostbiteVersion ?? "0.0.0";
    public static List<FileSystemSource> Sources => s_effectiveProfile?.Sources ?? new List<FileSystemSource>();
    public static string SdkFilename => s_effectiveProfile is null ? string.Empty : $"{s_effectiveProfile.InternalName}SDK";

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

    public static bool HasLoadedProfile => s_effectiveProfile is not null;

    public static readonly Dictionary<int, string> SharedBundles = new();

    private static Profile? s_effectiveProfile;
    private static bool s_profilesLoaded;
    private static readonly List<Profile> s_profiles = new();
    
    public static void Initialize()
    {
        if (Directory.Exists("Profiles"))
        {
            foreach (string file in Directory.EnumerateFiles("Profiles"))
            {
                Profile? profile;
                using (FileStream stream = new(file, FileMode.Open, FileAccess.Read))
                {
                    profile = JsonSerializer.Deserialize<Profile>(stream);
                }
                if (profile != null)
                {
                    s_profiles.Add(profile);
                }
            }
        }

        s_profilesLoaded = true;
    }

    public static bool Initialize(string profileKey)
    {
        if (IsInitialized)
        {
            return true;
        }
        if (!s_profilesLoaded)
        {
            Initialize();
        }
        s_effectiveProfile = s_profiles.Find(a => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase));
        if (s_effectiveProfile is not null)
        {
            foreach (string bundle in  s_effectiveProfile.SharedBundles)
            {
                SharedBundles.Add(HashString(bundle), bundle);
            }

            IsInitialized = true;
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

    public static string? GetDisplayName(string profileKey)
    {
        return s_profiles.Find(a => a.Name.Equals(profileKey, StringComparison.OrdinalIgnoreCase))?.DisplayName;
    }
}