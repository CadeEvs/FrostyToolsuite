using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUIDynamicTextureElementEntityData))]
	public class FBUIDynamicTextureElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUIDynamicTextureElementEntityData>
	{
		public new FrostySdk.Ebx.FBUIDynamicTextureElementEntityData Data => data as FrostySdk.Ebx.FBUIDynamicTextureElementEntityData;
		public override string DisplayName => "FBUIDynamicTextureElement";
		public BitmapImage Texture => texture;
		public Point4D UvRect => uvRect;

		protected BitmapImage texture;
		protected Point4D uvRect;

		protected Assets.TextureAsset textureAsset;

		public FBUIDynamicTextureElementEntity(FrostySdk.Ebx.FBUIDynamicTextureElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			var widgetParent = FindAncestor<UIWidgetEntity>();

			textureAsset = LoadedAssetManager.Instance.LoadAsset<Assets.TextureAsset>(this, Data.Texture);
			if (textureAsset != null)
			{
				texture = textureAsset.Texture;
				uvRect = new Point4D(0, 0, 1, 1);
			}
		}

		public override WidgetProxy CreateRenderProxy()
		{
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIWidgetsTextureElementProxy(this);
		}

        public override void OnDataModified()
        {
            base.OnDataModified();

			if (textureAsset != null)
			{
				LoadedAssetManager.Instance.UnloadAsset(textureAsset);
			}
			textureAsset = LoadedAssetManager.Instance.LoadAsset<Assets.TextureAsset>(this, Data.Texture);

			if (textureAsset != null)
			{
				texture = textureAsset.Texture;
				uvRect = new Point4D() { X = Data.UvRect.x, Y = Data.UvRect.y, Z = Data.UvRect.z, W = Data.UvRect.w };
			}
		}
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

			Data.UvRect = new FrostySdk.Ebx.Vec4() { x = 0, y = 0, z = 1, w = 1 };
			Data.Size = new FrostySdk.Ebx.Vec2() { x = 64, y = 64 };
		}
    }
}

