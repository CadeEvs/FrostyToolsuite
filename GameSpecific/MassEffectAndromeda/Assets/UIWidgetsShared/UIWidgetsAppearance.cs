using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using UIWidgets9SliceTextureData = FrostySdk.Ebx.UIWidgets9SliceTextureData;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsAppearance))]
    public class UIWidgetsAppearance : Asset, IAssetData<FrostySdk.Ebx.UIWidgetsAppearance>
    {
        public FrostySdk.Ebx.UIWidgetsAppearance Data => data as FrostySdk.Ebx.UIWidgetsAppearance;
        public FrostySdk.Ebx.UIWidgetsAppearanceBase Appearance => Data.Appearance.GetObjectAs<FrostySdk.Ebx.UIWidgetsAppearanceBase>();
        public bool HasTexture => texture != null;
        public BitmapImage Texture => texture;
        public Point4D UvRect => uvRect;
        public UIWidgets9SliceTextureData SliceData => sliceData;

        protected BitmapImage texture;
        protected Point4D uvRect;
        protected UIWidgets9SliceTextureData sliceData;

        public UIWidgetsAppearance(Guid fileGuid, FrostySdk.Ebx.UIWidgetsAppearance inData)
            : base(fileGuid, inData)
        {
        }

        public void InitializeResources(Entities.UIWidgetEntity widgetEntity)
        {
            if (Appearance is FrostySdk.Ebx.UIWidgetsTexturedAppearance)
            {
                var texturedAppearance = Appearance as FrostySdk.Ebx.UIWidgetsTexturedAppearance;
                texture = widgetEntity.GetTexture(texturedAppearance.Texture.Id, out uvRect);
                sliceData = texturedAppearance.SliceData;
            }
        }
    }
}
