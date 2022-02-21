using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Frosty.Controls
{
    public class FrostyDetachedTabControl : ContentControl
    {
        public static readonly DependencyProperty HeaderControlProperty = DependencyProperty.Register("HeaderControl", typeof(Control), typeof(FrostyDetachedTabControl), new FrameworkPropertyMetadata(null));
        public Control HeaderControl
        {
            get => (Control)GetValue(HeaderControlProperty);
            set
            {
                SetValue(HeaderControlProperty, value);

                Binding b = new Binding("SelectedContent")
                {
                    Source = value,
                    Mode = BindingMode.OneWay
                };
                BindingOperations.SetBinding(this, ContentProperty, b);
            }
        }
    }
}
