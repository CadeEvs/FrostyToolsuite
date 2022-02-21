using System.Windows;
using System.Windows.Controls;

namespace Frosty.Controls
{
    [TemplatePart(Name = PART_Watermark, Type = typeof(TextBlock))]
    public class FrostyWatermarkTextBox : TextBox
    {
        private const string PART_Watermark = "PART_Watermark";

        #region -- Properties --

        #region -- WatermarkText --

        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(FrostyWatermarkTextBox), new FrameworkPropertyMetadata(""));
        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        #endregion

        #endregion

        private TextBlock watermarkTextBlock;

        static FrostyWatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyWatermarkTextBox), new FrameworkPropertyMetadata(typeof(FrostyWatermarkTextBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            watermarkTextBlock = GetTemplateChild(PART_Watermark) as TextBlock;
            GotFocus += FrostyWatermarkTextBox_GotFocus;
            LostFocus += FrostyWatermarkTextBox_LostFocus;
        }

        private void FrostyWatermarkTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(Text == "")
                watermarkTextBlock.Visibility = Visibility.Visible;
        }

        private void FrostyWatermarkTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            watermarkTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
