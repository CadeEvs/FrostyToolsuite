using System.Collections.Generic;

namespace Frosty.Sdk.Profiles;

public class Profile
{
    public string Name  { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public string InternalName { get; set; } = string.Empty;

    public string TypeInfoSignature { get; set; } = string.Empty;
    public bool HasStrippedTypeNames { get; set; }

    public int DataVersion { get; set; }
    public string FrostbiteVersion { get; set; } = "0.0.0";
    
    public List<FileSystemSource> Sources { get; set; } = new();

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

    public string DefaultDiffuse { get; set; } = string.Empty;
    public string DefaultNormals { get; set; } = string.Empty;
    public string DefaultMask { get; set; } = string.Empty;
    public string DefaultTint { get; set; } = string.Empty;

    public List<string> SharedBundles { get; set; } = new();
}