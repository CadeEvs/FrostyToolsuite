using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsTextureElementEntityData))]
	public class UIWidgetsTextureElementEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsTextureElementEntityData>
	{
		protected readonly int Property_InputTexture = Frosty.Hash.Fnv1.HashString("InputTexture");
		protected readonly int Property_TextureId = Frosty.Hash.Fnv1.HashString("TextureId");

		public new FrostySdk.Ebx.UIWidgetsTextureElementEntityData Data => data as FrostySdk.Ebx.UIWidgetsTextureElementEntityData;
		public override string DisplayName => "UIWidgetsTextureElement";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("InputTexture", Direction.In, typeof(Assets.TextureAsset)));
				outProperties.Add(new ConnectionDesc("TextureId", Direction.In, typeof(CString)));
				return outProperties;
            }
        }
        public BitmapImage Texture => texture;
		public Point4D UvRect => uvRect;
		
		protected BitmapImage texture;
		protected Point4D uvRect;

		protected Property<Assets.TextureAsset> inputTextureProperty;
		protected Property<CString> textureIdProperty;

		public UIWidgetsTextureElementEntity(FrostySdk.Ebx.UIWidgetsTextureElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inputTextureProperty = new Property<Assets.TextureAsset>(this, Property_InputTexture);
			textureIdProperty = new Property<CString>(this, Property_TextureId, Data.TextureId);

			var widgetParent = FindAncestor<UIWidgetEntity>();

			int textureId = Data.Texture.Id;
			if (textureId == 0)
			{
				textureId = Frosty.Hash.Fnv1.HashString(Data.TextureId);
			}
			if (textureId != 0)
			{
				texture = widgetParent.GetTexture(textureId, out uvRect);
			}
		}

        public override WidgetProxy CreateRenderProxy()
        {
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIWidgetsTextureElementProxy(this);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inputTextureProperty.NameHash)
			{
				var textureAsset = inputTextureProperty.Value;
				if (textureAsset != null)
				{
					texture = inputTextureProperty.Value.Texture;
					uvRect = new Point4D(0, 0, 1, 1);
				}
				return;
			}
			else if (propertyHash == textureIdProperty.NameHash)
			{
				var widgetParent = FindAncestor<UIWidgetEntity>();
				int textureId = Frosty.Hash.Fnv1.HashString(textureIdProperty.Value);

				texture = widgetParent.GetTexture(textureId, out uvRect);
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnDataModified()
        {
            base.OnDataModified();

			var widgetParent = FindAncestor<UIWidgetEntity>();
			int textureId = Data.Texture.Id;

			if (textureId == 0)
			{
				textureId = Frosty.Hash.Fnv1.HashString(Data.TextureId);
			}
			if (textureId != 0)
			{
				texture = widgetParent.GetTexture(textureId, out uvRect);
			}
		}

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

			Data.Texture.SliceData = new FrostySdk.Ebx.UIWidgets9SliceTextureData() { Right = 0, Left = 1, Top = 0, Bottom = 1 };
			Data.UvRect = new FrostySdk.Ebx.Vec4() { x = 0, y = 0, z = 1, w = 1 };
			Data.Size = new FrostySdk.Ebx.Vec2() { x = 64, y = 64 };
        }
    }
}

