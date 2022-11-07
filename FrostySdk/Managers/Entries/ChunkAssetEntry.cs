using System;

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
        public bool IsTocChunk;
        public bool TocChunkSpecialHack;
    }
}