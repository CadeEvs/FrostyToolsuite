using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUIStaticTextureElementEntityData))]
	public class FBUIStaticTextureElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUIStaticTextureElementEntityData>
	{
		public new FrostySdk.Ebx.FBUIStaticTextureElementEntityData Data => data as FrostySdk.Ebx.FBUIStaticTextureElementEntityData;
		public override string DisplayName => "FBUIStaticTextureElement";
		public BitmapImage Texture => texture;
		public Point4D UvRect => uvRect;

		protected BitmapImage texture;
		protected Point4D uvRect;

		public FBUIStaticTextureElementEntity(FrostySdk.Ebx.FBUIStaticTextureElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			var widgetParent = FindAncestor<UIWidgetEntity>();

			int textureId = Data.Texture.Id;
			texture = widgetParent.GetTexture(textureId, out uvRect);
		}

		public override WidgetProxy CreateRenderProxy()
		{
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIWidgetsTextureElementProxy(this);
		}

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

			Data.UvRect = new FrostySdk.Ebx.Vec4() { x = 0, y = 0, z = 1, w = 1 };
			Data.Size = new FrostySdk.Ebx.Vec2() { x = 64, y = 64 };
		}
    }
}

