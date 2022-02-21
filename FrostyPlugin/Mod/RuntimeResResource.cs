using FrostySdk.Managers;

namespace Frosty.Core.Mod
{
    public sealed class RuntimeResResource : ResResource
    {
        public RuntimeResResource(ResAssetEntry entry)
            : base(entry)
        {
            resourceIndex = -1;
            name = entry.Name;
            sha1 = entry.Sha1;
            size = entry.Size;
            resType = entry.ResType;
            resRid = entry.ResRid;
            resMeta = entry.ResMeta;
        }
    }
}
