using System.Collections.Generic;

namespace Frosty.Sdk.Profiles;

public class Profile
{
    public string Name  { get; set; }
    public string DisplayName { get; set; }

    public string CacheName { get; set; }
    public string Deobfuscator { get; set; }
    public string AssetLoader { get; set; }

    public int DataVersion { get; set; }
    public List<FileSystemSource> Sources { get; set; }
    public string SdkFileName { get; set; }
    public string BannerPath { get; set; }

    public int EbxVersion { get; set; }
    public bool RequiresKey { get; set; }
    public bool MustAddChunks { get; set; }
    public bool EnableExecution { get; set; }
    public bool HasAntiCheat { get; set; }

    public byte EbxCompression { get; set; }
    public byte ResCompression { get; set; }
    public byte ChunkCompression { get; set; }
    public byte TextureChunkCompression { get; set; }
    public int MaxBufferSize { get; set; }
    public int ZStdCompressionLevel { get; set; }

    public string DefaultDiffuse { get; set; }
    public string DefaultNormals { get; set; }
    public string DefaultMask { get; set; }
    public string DefaultTint { get; set; }

    public List<string> SharedBundles { get; set; }
}