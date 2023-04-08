using System;
using System.Collections.Generic;

namespace Frosty.Sdk.Managers.Entries;

public class ChunkAssetEntry : AssetEntry
{
    public override string Name => Id.ToString();
    public override string Type => "Chunk";
    public override string AssetType => "chunk";

    /// <summary>
    /// Id of this chunk.
    /// </summary>
    public Guid Id;
    
    /// <summary>
    /// Offset of the FirstMip if this is a chunk for a texture, else it's 0.
    /// </summary>
    public uint LogicalOffset;
    
    /// <summary>
    /// Size of the chunk from the FirstMip if this is a chunk for a texture, else it's the size of the chunk.
    /// </summary>
    public uint LogicalSize;

    /// <summary>
    /// List of SuperBundles the chunk is in.
    /// </summary>
    public readonly List<int> SuperBundles = new();

    // these are only used in the mod applying process
    // we only set them when modifying an asset
    
    /// <summary>
    /// Hash of the asset referencing this chunk.
    /// </summary>
    public int H32;
    
    /// <summary>
    /// First mip that is stored in the bundle version of a chunk for a texture, else its -1.
    /// </summary>
    public int FirstMip = -1;
    
    /// <summary>
    /// Offset of the FirstMip in the compressed chunk if this is a chunk for a texture, else it's 0.
    /// </summary>
    public uint RangeStart;
    
    /// <summary>
    /// Size of the compressed chunk from the FirstMip if this is a chunk for a texture, else it's the size of the compressed chunk.
    /// </summary>
    public uint RangeEnd;
    
    /// <summary>
    /// List of SuperBundles the chunk is added to.
    /// </summary>
    public readonly List<int> AddedSuperBundles = new();

    public ChunkAssetEntry(Guid inChunkId, Sha1 inSha1, long inSize, uint inLogicalOffset, uint inLogicalSize, params int[] superBundleIds)
        : base(inSha1, inSize, inLogicalOffset + inLogicalSize)
    {
        Id = inChunkId;
        LogicalOffset = inLogicalOffset;
        LogicalSize = inLogicalSize;
        SuperBundles.AddRange(superBundleIds);
    }

    public override bool AddToBundle(int bid)
    {
        if (Bundles.Count == 0 && SuperBundles.Count > 0)
        {
            return false;
        }

        return base.AddToBundle(bid);
    }

    /// <summary>
    /// Adds the current asset to the specified SuperBundle.
    /// </summary>
    /// <param name="sbId">The id of the SuperBundle to add to.</param>
    /// <returns>True if it was successfully added.</returns>
    public bool AddToSuperBundle(int sbId)
    {
        if (IsInSuperBundle(sbId))
        {
            return false;
        }

        AddedSuperBundles.Add(sbId);
        IsDirty = true;

        return true;
    }

    /// <summary>
    /// Check if this Asset is in a SuperBundle or was added to it.
    /// </summary>
    /// <param name="sbId">The id of the SuperBundle to check.</param>
    /// <returns>True if the Asset is in the SuperBundle or was added to it.</returns>
    public bool IsInSuperBundle(int sbId) => SuperBundles.Contains(sbId) || AddedSuperBundles.Contains(sbId);
}