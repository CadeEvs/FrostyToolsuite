using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Mod
{
    public sealed class RuntimeResResource : ResResource
    {
        public RuntimeResResource(ResAssetEntry entry)
            : base(entry)
        {
            resourceIndex = -1;
            name = entry.Name.ToLower();
            sha1 = entry.Sha1;
            size = entry.OriginalSize;
            resType = entry.ResType;
            resRid = entry.ResRid;
            resMeta = entry.ResMeta;
        }
    }
}
