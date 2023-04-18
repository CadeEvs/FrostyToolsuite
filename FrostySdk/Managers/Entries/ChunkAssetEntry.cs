using System;
using System.Collections.Generic;
using System.Linq;

namespace Frosty.Sdk.Managers.Entries;

public class ChunkAssetEntry : AssetEntry
{
    public override string Name => Id.ToString();
    
    public override string Type => "Chunk";
    
    public override string AssetType => "chunk";

    /// <summary>
    /// Id of this chunk.
    /// </summary>
    public Guid Id { get; }
    
    /// <summary>
    /// Offset of the FirstMip if this is a chunk for a texture, else it's 0.
    /// </summary>
    public uint LogicalOffset { get; internal set; }
    
    /// <summary>
    /// Size of the chunk from the FirstMip if this is a chunk for a texture, else it's the size of the chunk.
    /// </summary>
    public uint LogicalSize { get; internal set; }

    /// <summary>
    /// SuperBundles that contain this <see cref="ChunkAssetEntry"/>.
    /// </summary>
    public readonly HashSet<int> SuperBundles = new();

    // these are not assigned by the AssetLoader, but are needed for rewriting of the asset
    
    /// <summary>
    /// Hash of the asset referencing this chunk.
    /// </summary>
    public int H32 { get; }
    
    /// <summary>
    /// First mip that is stored in the bundle version of a chunk for a texture, else its -1.
    /// </summary>
    public int FirstMip { get; } = -1;
    
    /// <summary>
    /// Offset of the FirstMip in the compressed chunk if this is a chunk for a texture, else it's 0.
    /// </summary>
    public uint RangeStart { get; }
    
    /// <summary>
    /// Size of the compressed chunk from the FirstMip if this is a chunk for a texture, else it's the size of the compressed chunk.
    /// </summary>
    public uint RangeEnd { get; }

    public ChunkAssetEntry(Guid inChunkId, Sha1 inSha1, long inSize, uint inLogicalOffset, uint inLogicalSize, params int[] superBundleIds)
        : base(inSha1, inSize, inLogicalOffset + inLogicalSize)
    {
        Id = inChunkId;
        LogicalOffset = inLogicalOffset;
        LogicalSize = inLogicalSize;
        SuperBundles.UnionWith(superBundleIds);
    }

    public ChunkAssetEntry(Guid inChunkId, Sha1 inSha1, long inSize, uint inLogicalOffset, uint inLogicalSize,
        uint inRangeStart, uint inRangeEnd, int inH32, int inFirstMip, params int[] superBundleIds)
        : base(inSha1, inSize, inLogicalOffset + inLogicalSize)
    {
        Id = inChunkId;
        LogicalOffset = inLogicalOffset;
        LogicalSize = inLogicalSize;
        RangeStart = inRangeStart;
        RangeEnd = inRangeEnd;
        H32 = inH32;
        FirstMip = inFirstMip;
        SuperBundles.UnionWith(superBundleIds);
    }

    /// <summary>
    /// Check if this Asset is in a SuperBundle or was added to it.
    /// </summary>
    /// <param name="sbId">The id of the SuperBundle to check.</param>
    /// <returns>True if the Asset is in the SuperBundle.</returns>
    public bool IsInSuperBundle(int sbId) => SuperBundles.Contains(sbId);
}