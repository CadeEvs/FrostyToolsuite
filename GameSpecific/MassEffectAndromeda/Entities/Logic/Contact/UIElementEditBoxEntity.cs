using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementEditBoxEntityData))]
	public class UIElementEditBoxEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIElementEditBoxEntityData>
	{
		public new FrostySdk.Ebx.UIElementEditBoxEntityData Data => data as FrostySdk.Ebx.UIElementEditBoxEntityData;
		public override string DisplayName => "UIElementEditBox";
		public Assets.UIElementFontStyle FontStyle => fontStyle;

		protected Assets.UIElementFontStyle fontStyle;

		public UIElementEditBoxEntity(FrostySdk.Ebx.UIElementEditBoxEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			fontStyle = LoadedAssetManager.Instance.LoadAsset<Assets.UIElementFontStyle>(this, Data.FontStyle);
		}

		public override WidgetProxy CreateRenderProxy()
		{
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIElementEditBoxProxy(this);
		}

		public override void Destroy()
		{
			LoadedAssetManager.Instance.UnloadAsset(fontStyle);
			base.Destroy();
		}
	}
}

