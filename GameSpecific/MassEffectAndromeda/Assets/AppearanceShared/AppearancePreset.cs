using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
#if MASS_EFFECT
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.AppearancePreset))]
    public class AppearancePreset : Asset, IAssetData<FrostySdk.Ebx.AppearancePreset>
    {
        public FrostySdk.Ebx.AppearancePreset Data => data as FrostySdk.Ebx.AppearancePreset;
        public IEnumerable<CommonAppearanceItem> Items => appearanceItems.Values;

        private Dictionary<Guid, CommonAppearanceItem> appearanceItems = new Dictionary<Guid, CommonAppearanceItem>();

        public AppearancePreset(Guid fileGuid, FrostySdk.Ebx.AppearancePreset inData)
            : base(fileGuid, inData)
        {
            foreach (var itemRef in Data.Items)
            {
                var item = itemRef.GetObjectAs<FrostySdk.Ebx.AppearancePresetItem>();
                CommonAppearanceItem appearanceItem = null;

                if (item is FrostySdk.Ebx.RandomAppearancePresetItem)
                {
                    var randomPresetItem = item as FrostySdk.Ebx.RandomAppearancePresetItem;

                    Random r = new Random();
                    int randidx = r.Next(0, randomPresetItem.RandomItems.Count);

                    appearanceItem = LoadedAssetManager.Instance.LoadAsset<CommonAppearanceItem>(this, randomPresetItem.RandomItems[randidx]);
                }
                else
                {
                    appearanceItem = LoadedAssetManager.Instance.LoadAsset<CommonAppearanceItem>(this, item.Item);
                }

                if (appearanceItem != null)
                {
                    var appearanceSlot = appearanceItem.Data.Slot.External.FileGuid;

                    if (appearanceItems.ContainsKey(appearanceSlot))
                    {
                        LoadedAssetManager.Instance.UnloadAsset(appearanceItems[appearanceSlot]);
                        appearanceItems.Remove(appearanceSlot);
                    }

                    appearanceItems.Add(appearanceSlot, appearanceItem);
                }
            }
        }

        public override void Dispose()
        {
            foreach (var item in appearanceItems.Values)
                LoadedAssetManager.Instance.UnloadAsset(item);
        }
    }
#endif
}
