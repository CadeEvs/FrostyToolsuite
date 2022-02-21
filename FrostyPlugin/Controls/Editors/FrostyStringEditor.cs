using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyStringEditor : FrostyTypeEditor<TextBox>
    {
        public FrostyStringEditor()
        {
            ValueProperty = TextBox.TextProperty;
        }

        protected override void CustomizeEditor(TextBox editor, FrostyPropertyGridItemData item)
        {
            base.CustomizeEditor(editor, item);
            editor.Padding = new Thickness(0);
            editor.Background = new SolidColorBrush(new Color() { A = 0, R = 0, G = 0, B = 0 });
            editor.Height = 22;
            editor.VerticalContentAlignment = VerticalAlignment.Center;
            editor.Margin = new Thickness(-2, 0, 0, 0);
            editor.GotKeyboardFocus += (s, o) => { editor.SelectAll(); };
        }
    }
}
