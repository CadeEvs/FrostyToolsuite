using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrostySdk.Managers.Entries;

namespace TestPlugin.TypeOverrides
{
    public class SubworldTypeOverride : BaseTypeOverride
    {
        [EbxFieldMeta(EbxFieldType.Pointer, "SubWorldData")]
        public PointerRef Bundle { get; set; } = new PointerRef();

        [IsHidden]
        public BaseFieldOverride BundleName { get; set; }

        public override void Load()
        {
            dynamic subWorld = Original;
            EbxAssetEntry bundleEntry = App.AssetManager.GetEbxEntry(subWorld.BundleName);
            EbxAsset bundleAsset = App.AssetManager.GetEbx(bundleEntry);

            Bundle = new PointerRef(new EbxImportReference() { FileGuid = bundleEntry.Guid, ClassGuid = bundleAsset.RootInstanceGuid });
        }

        public override void Save(object e)
        {
            ItemModifiedEventArgs item = e as ItemModifiedEventArgs;

            if (item.Item.Name == "Bundle")
            {
                ((dynamic)Original).BundleName = App.AssetManager.GetEbxEntry(Bundle.External.FileGuid).Name;
            }
        }
    }
}
