using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using FrostySdk.Managers.Entries;
using Frosty.Core.IO;
using System.Collections.Generic;

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
        protected List<int> superBundlesToAdd = new List<int>();

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
            if ((reader as FrostyModReader).Version > 5)
            {
                int sbCount = reader.ReadInt();
                for (int i = 0; i < sbCount; i++)
                {
                    superBundlesToAdd.Add(reader.ReadInt());
                }
            }
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
            chunkEntry.AddedSuperBundles.AddRange(superBundlesToAdd);

            if (chunkEntry.FirstMip == -1 && chunkEntry.RangeStart != 0)
                chunkEntry.FirstMip = 0;
        }
    }
}
