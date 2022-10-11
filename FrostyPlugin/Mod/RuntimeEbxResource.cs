using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Mod
{
    public sealed class RuntimeEbxResource : EbxResource
    {
        public RuntimeEbxResource(EbxAssetEntry entry)
            : base(entry)
        {
            resourceIndex = -1;
            name = entry.Name.ToLower();
            sha1 = entry.Sha1;
            size = entry.OriginalSize;
        }
    }
}
