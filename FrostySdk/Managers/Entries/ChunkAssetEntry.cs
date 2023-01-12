using System;
using System.Collections.Generic;

namespace FrostySdk.Managers.Entries
{
    public class ChunkAssetEntry : AssetEntry
    {
        public override string Name => Id.ToString();
        public override string Type => "Chunk";
        public override string AssetType => "chunk";

        public Guid Id;
        public uint BundledSize;
        public uint LogicalOffset;
        public uint LogicalSize;
        public uint RangeStart;
        public uint RangeEnd;

        public int H32;
        public int FirstMip = -1;
        public bool TocChunkSpecialHack;
        public List<int> SuperBundles = new List<int>();
        public List<int> AddedSuperBundles = new List<int>();

        /// <summary>
        /// Adds the current asset to the specified superbundle
        /// </summary>
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
        /// Returns true if asset is in the specified superbundle
        /// </summary>
        public bool IsInSuperBundle(int sbId) => SuperBundles.Contains(sbId) || AddedSuperBundles.Contains(sbId);
    }
}