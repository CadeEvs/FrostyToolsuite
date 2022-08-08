using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsArtEntityData))]
	public class UIWidgetsArtEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsArtEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsArtEntityData Data => data as FrostySdk.Ebx.UIWidgetsArtEntityData;
		public override string DisplayName => "UIWidgetsArt";
		public FrostySdk.Ebx.UIWidgetsAppearanceBase Appearance => appearanceAsset.Appearance;

		protected Assets.UIWidgetsAppearance appearanceAsset;

		public UIWidgetsArtEntity(FrostySdk.Ebx.UIWidgetsArtEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			appearanceAsset = LoadedAssetManager.Instance.LoadAsset<Assets.UIWidgetsAppearance>(this, Data.Appearance);
			appearanceAsset.InitializeResources(FindAncestor<UIWidgetEntity>());
		}

        public override WidgetProxy CreateRenderProxy()
        {
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIWidgetsArtProxy(this);
        }

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(appearanceAsset);
            base.Destroy();
        }
    }
}

