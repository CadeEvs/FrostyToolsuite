using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Entities;
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
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SimpleAppearanceItemData))]
    public class SimpleAppearanceItem : CommonAppearanceItem, IAssetData<FrostySdk.Ebx.SimpleAppearanceItemData>
    {
        public new FrostySdk.Ebx.SimpleAppearanceItemData Data => data as FrostySdk.Ebx.SimpleAppearanceItemData;

        public Blueprint Blueprint { get; private set; }

        public SimpleAppearanceItem(Guid fileGuid, FrostySdk.Ebx.SimpleAppearanceItemData inData)
            : base(fileGuid, inData)
        {
            Blueprint = LoadedAssetManager.Instance.LoadAsset<Blueprint>(this, Data.Blueprint);
            if (Blueprint == null)
            {
                foreach (var bundleRefPair in Data.BlueprintBundleRefAssetNamePairs)
                {
                    var blueprint = LoadedAssetManager.Instance.LoadAsset<Blueprint>(bundleRefPair.AssetName);
                    if (blueprint is ObjectBlueprint)
                    {
                        Blueprint = blueprint;
                        break;
                    }
                }
            }
        }

        public override FrostySdk.Ebx.GameObjectData GenerateEntityData()
        {
            if (Blueprint == null)
                return null;

            return Blueprint.CreateEntityData();
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(Blueprint);
        }
    }
#endif
}
