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

namespace TestPlugin.TypeOverrides
{
    public class UnlockDataCollectionTypeOverride : BaseTypeOverride
    {
        [EbxFieldMeta(EbxFieldType.Pointer, "UnlockAsset")]
        public List<PointerRef> Unlocks { get; set; } = new List<PointerRef>();
        [EbxFieldMeta(EbxFieldType.Pointer, "UnlockAsset")]
        public List<PointerRef> ArcadeUnlocks { get; set; } = new List<PointerRef>();

        [IsHidden]
        public BaseFieldOverride UnlockGuids { get; set; }
        [IsHidden]
        public BaseFieldOverride ArcadeUnlockGuids { get; set; }

        private static Dictionary<Guid, PointerRef> mapping = new Dictionary<Guid, PointerRef>();

        public override void Load()
        {
            if (mapping.Count == 0)
            {
                foreach (var entry in App.AssetManager.EnumerateEbx())
                {
                    if (TypeLibrary.IsSubClassOf(entry.Type, "UnlockAssetBase"))
                    {
                        var ebxAsset = App.AssetManager.GetEbx(entry);
                        mapping.Add(ebxAsset.RootInstanceGuid, new PointerRef(new EbxImportReference() { FileGuid = entry.Guid, ClassGuid = ebxAsset.RootInstanceGuid }));
                    }
                }
            }

            dynamic unlockCollection = Original;
            foreach (var guid in unlockCollection.UnlockGuids)
            {
                if (mapping.ContainsKey(guid))
                {
                    Unlocks.Add(mapping[guid]);
                }
                else
                {
                    Unlocks.Add(new PointerRef());
                }
            }
            foreach (var guid in unlockCollection.ArcadeUnlockGuids)
            {
                if (mapping.ContainsKey(guid))
                {
                    ArcadeUnlocks.Add(mapping[guid]);
                }
                else
                {
                    ArcadeUnlocks.Add(new PointerRef());
                }
            }
        }

        public override void Save(object e)
        {
            // @todo: finish save function

            ItemModifiedEventArgs item = e as ItemModifiedEventArgs;
            if (item.Item.Parent.Name == "Unlocks" || item.Item.Name == "Unlocks")
            {
                if (item.ModifiedArgs.Type == ItemModifiedTypes.Add)
                {
                    //unlockCollection.UnlockGuids.add(((PointerRef)item.NewValue).External.ClassGuid);
                    ((dynamic)Original).UnlockGuids.Add(Guid.NewGuid());
                }
                else if (item.ModifiedArgs.Type == ItemModifiedTypes.Assign)
                {
                    
                }
            }
            else if (item.Item.Parent.Name == "ArcadeUnlocks")
            {
            }
        }
    }
}
