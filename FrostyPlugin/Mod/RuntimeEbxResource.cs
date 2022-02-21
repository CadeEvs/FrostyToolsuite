using FrostySdk.Managers;

namespace Frosty.Core.Mod
{
    public sealed class RuntimeEbxResource : EbxResource
    {
        public RuntimeEbxResource(EbxAssetEntry entry)
            : base(entry)
        {
            resourceIndex = -1;
            name = entry.Name;
            sha1 = entry.Sha1;
            size = entry.Size;
        }
    }
}
