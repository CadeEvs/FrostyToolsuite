using FrostySdk.Interfaces;
using FrostySdk.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Frosty.Core.Controls;
using Frosty.Core;

namespace LegacyTextPlugin
{
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class LegacyTextEditor : FrostyAssetEditor
    {
        private const string PART_TextBox = "PART_TextBox";

        private TextBox tb;
        private bool firstTimeLoad = true;

        static LegacyTextEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegacyTextEditor), new FrameworkPropertyMetadata(typeof(LegacyTextEditor)));
        }

        public LegacyTextEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            tb = GetTemplateChild(PART_TextBox) as TextBox;
            Loaded += LegacyTextEditor_Loaded;
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>();
        }

        private void LegacyTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                byte[] buffer = null;
                using (NativeReader reader = new NativeReader(App.AssetManager.GetCustomAsset("legacy", AssetEntry)))
                    buffer = reader.ReadToEnd();
                tb.Text = Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
