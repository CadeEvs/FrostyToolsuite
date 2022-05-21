using FrostySdk.Managers;

namespace Frosty.Core.Mod
{
    public class FsFileResource : BaseModResource
    {
        public override ModResourceType Type => ModResourceType.FsFile;

        public FsFileResource()
        {
        }

        internal FsFileResource(AssetEntry entry)
            : base(entry)
        {
        }
    }
}
