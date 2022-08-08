using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.UIElementFontStyle))]
    public class UIElementFontStyle : Asset, IAssetData<FrostySdk.Ebx.UIElementFontStyle>
    {
        public FrostySdk.Ebx.UIElementFontStyle Data => data as FrostySdk.Ebx.UIElementFontStyle;
        public FontFamily FontFamily => fontAsset.FontFamily;
        public float FontSize => fontSize;

        protected UITtfAsset fontAsset;
        protected float fontSize;

        public UIElementFontStyle(Guid fileGuid, FrostySdk.Ebx.UIElementFontStyle inData)
            : base(fileGuid, inData)
        {
            var fontDefinition = Data.Hd.GetObjectAs<FrostySdk.Ebx.UIElementFontDefinition>();

            fontAsset = LoadedAssetManager.Instance.LoadAsset<UITtfAsset>(fontDefinition.FontLookup[0].FontAssetPath);
            fontSize = fontDefinition.PointSize;
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(fontAsset);
            base.Dispose();
        }
    }
}
