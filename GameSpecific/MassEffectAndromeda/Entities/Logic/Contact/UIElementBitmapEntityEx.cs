using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementBitmapEntityExData))]
	public class UIElementBitmapEntityEx : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIElementBitmapEntityExData>
	{
		protected readonly int Property_DynamicTextureId = Frosty.Hash.Fnv1.HashString("DynamicTextureId");

		public new FrostySdk.Ebx.UIElementBitmapEntityExData Data => data as FrostySdk.Ebx.UIElementBitmapEntityExData;
		public override string DisplayName => "UIElementBitmapEntityEx";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("DynamicTextureId", Direction.In, typeof(CString)));
				return outProperties;
            }
        }
        public BitmapImage Texture => texture;
		public Point4D UvRect => uvRect;

		protected BitmapImage texture;
		protected Point4D uvRect;

		protected Property<CString> dynamicTextureIdProperty;

		public UIElementBitmapEntityEx(FrostySdk.Ebx.UIElementBitmapEntityExData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			dynamicTextureIdProperty = new Property<CString>(this, Property_DynamicTextureId, Data.DynamicTextureId);
			var widgetParent = FindAncestor<UIWidgetEntity>();

			int textureId = Frosty.Hash.Fnv1.HashString(Data.TextureId);
			texture = widgetParent.GetTexture(textureId, out uvRect);
		}

		public override WidgetProxy CreateRenderProxy()
		{
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new UIWidgetsTextureElementProxy(this);
		}

        public override void AddPropertyConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            base.AddPropertyConnection(srcPort, dstObject, dstPort);
        }

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == dynamicTextureIdProperty.NameHash)
			{
				var widgetParent = FindAncestor<UIWidgetEntity>();
				int textureId = Frosty.Hash.Fnv1.HashString(dynamicTextureIdProperty.Value);

				texture = widgetParent.GetTexture(textureId, out uvRect);
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}

