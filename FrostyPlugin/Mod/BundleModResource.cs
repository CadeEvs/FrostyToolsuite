using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Mod
{
    public sealed class BundleResource : BaseModResource
    {
        public override ModResourceType Type => ModResourceType.Bundle;
        private int superBundleName;

        public BundleResource()
        {
        }

        public override void Read(NativeReader reader)
        {
            base.Read(reader);
            name = reader.ReadNullTerminatedString();
            superBundleName = reader.ReadInt();
        }

        public override void FillAssetEntry(object entry)
        {
            BundleEntry bentry = entry as BundleEntry;
            bentry.Name = name;
            bentry.SuperBundleId = superBundleName;
        }
    }
}
