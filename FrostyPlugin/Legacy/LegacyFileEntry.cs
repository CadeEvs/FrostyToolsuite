using Frosty.Hash;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;

namespace Frosty.Core.Legacy
{
    public sealed class LegacyFileEntry : AssetEntry
    {
        public class ChunkCollectorInstance
        {
            public ChunkCollectorInstance ModifiedEntry { get; set; }
            public bool IsModified => ModifiedEntry != null;

            public EbxAssetEntry Entry;
            public Guid ChunkId;
            public long Offset;
            public long CompressedOffset;
            public long CompressedSize;
            public long Size;
        }

        public Guid ChunkId
        {
            get
            {
                if (CollectorInstances.Count == 0)
                    return Guid.Empty;

                return CollectorInstances[0].IsModified ? CollectorInstances[0].ModifiedEntry.ChunkId : CollectorInstances[0].ChunkId;
            }
        }

        public int NameHash => Fnv1.HashString(Name);

        public override string AssetType => "legacy";

        public override string Type
        {
            get
            {
                int lastPeriodIndex = Name.LastIndexOf('.');
                return lastPeriodIndex == -1 ? "" : Name.Substring(lastPeriodIndex + 1).ToUpper();
            }
            set { }
        }

        public override string Filename
        {
            get
            {
                int lastPeriodIndex = base.Filename.LastIndexOf('.');
                return lastPeriodIndex == -1 ? base.Filename : base.Filename.Substring(0, lastPeriodIndex);
            }
        }

        public override bool IsModified => CollectorInstances.Count != 0 && CollectorInstances[0].IsModified;

        public override bool IsDirty => App.AssetManager.GetChunkEntry(ChunkId).IsDirty;

        public override void ClearModifications()
        {
            foreach (ChunkCollectorInstance inst in CollectorInstances)
                inst.ModifiedEntry = null;
        }

        public List<ChunkCollectorInstance> CollectorInstances = new List<ChunkCollectorInstance>();
    }
}
