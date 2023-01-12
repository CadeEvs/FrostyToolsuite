using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Mod
{
    public sealed class RuntimeChunkResource : ChunkResource
    {
        public RuntimeChunkResource(ChunkAssetEntry entry)
            : base(entry)
        {
            resourceIndex = -1;
            name = entry.Id.ToString();
            sha1 = entry.Sha1;
            size = entry.OriginalSize;
            logicalOffset = entry.LogicalOffset;
            logicalSize = entry.LogicalSize;
            rangeStart = entry.RangeStart;
            rangeEnd = entry.RangeEnd;
            h32 = entry.H32;
            firstMip = entry.FirstMip;
            superBundlesToAdd.AddRange(entry.AddedSuperBundles);
        }
    }
}
