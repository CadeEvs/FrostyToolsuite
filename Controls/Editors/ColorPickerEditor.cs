using Frosty.Core.Controls.Editors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls.Editors
{
    public class ColorPickerEditor : FrostyTypeEditor<ColorPickerControl>
    {
        public ColorPickerEditor()
        {
            ValueProperty = ColorPickerControl.ResultProperty;
        }
    }

    public class ColorPickerControl : Control
    {
        public static readonly DependencyProperty SelectedColorTextProperty = DependencyProperty.Register("SelectedColorText", typeof(string), typeof(ColorPickerControl), new FrameworkPropertyMetadata("", OnSelectedColorTextChanged));
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(ColorPickerControl), new FrameworkPropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof(FrostySdk.Ebx.Vec4), typeof(ColorPickerControl), new FrameworkPropertyMetadata(null));

        public string SelectedColorText
        {
            get => (string)GetValue(SelectedColorTextProperty);
            set => SetValue(SelectedColorTextProperty, value);
        }
        public Brush SelectedColor
        {
            get => (Brush)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
        public FrostySdk.Ebx.Vec4 Result
        {
            get => (FrostySdk.Ebx.Vec4)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }

        static ColorPickerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerControl), new FrameworkPropertyMetadata(typeof(ColorPickerControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SelectedColorText = $"{((int)(Result.x * 255.0f)).ToString("x2")}{((int)(Result.y * 255.0f)).ToString("x2")}{((int)(Result.z * 255.0f)).ToString("x2")}";
            SelectedColor = new SolidColorBrush(Color.FromScRgb(1.0f, Result.x, Result.y, Result.z));
        }

        private static void OnSelectedColorTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerControl ctrl = d as ColorPickerControl;
            string colorText = (string)e.NewValue;

            if (colorText.Length < 6)
                return;

            string rText = colorText.Substring(0, 2);
            string gText = colorText.Substring(2, 2);
            string bText = colorText.Substring(4, 2);

            byte r, g, b;

            if (!byte.TryParse(rText, NumberStyles.HexNumber, null, out r))
                return;
            if (!byte.TryParse(gText, NumberStyles.HexNumber, null, out g))
                return;
            if (!byte.TryParse(bText, NumberStyles.HexNumber, null, out b))
                return;

            ctrl.SelectedColor = new SolidColorBrush(Color.FromArgb(255, r, g, b));
            ctrl.Result = new FrostySdk.Ebx.Vec4() { x = r / 255.0f, y = g / 255.0f, z = b / 255.0f, w = 1.0f };
        }
    }
}
