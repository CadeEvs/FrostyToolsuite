using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Mod
{
    public class ChunkResource : BaseModResource
    {
        public override ModResourceType Type => ModResourceType.Chunk;

        protected uint rangeStart;
        protected uint rangeEnd;
        protected uint logicalOffset;
        protected uint logicalSize;
        protected int h32;
        protected int firstMip;

        public ChunkResource()
        {
        }

        internal ChunkResource(ChunkAssetEntry entry)
            : base(entry)
        {
        }

        public override void Read(NativeReader reader)
        {
            base.Read(reader);

            rangeStart = reader.ReadUInt();
            rangeEnd = reader.ReadUInt();
            logicalOffset = reader.ReadUInt();
            logicalSize = reader.ReadUInt();
            h32 = reader.ReadInt();
            firstMip = reader.ReadInt();
        }

        public override void FillAssetEntry(object entry)
        {
            base.FillAssetEntry(entry);
            ChunkAssetEntry chunkEntry = entry as ChunkAssetEntry;

            chunkEntry.Id = new Guid(name);
            chunkEntry.RangeStart = rangeStart;
            chunkEntry.RangeEnd = rangeEnd;
            chunkEntry.LogicalOffset = logicalOffset;
            chunkEntry.LogicalSize = logicalSize;
            chunkEntry.H32 = h32;
            chunkEntry.FirstMip = firstMip;
            chunkEntry.IsTocChunk = IsTocChunk;

            if (chunkEntry.FirstMip == -1 && chunkEntry.RangeStart != 0)
                chunkEntry.FirstMip = 0;
        }
    }
}
