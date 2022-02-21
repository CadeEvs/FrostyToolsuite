using System.Windows;
using System.Windows.Controls;

namespace Frosty.Controls
{
    public class FrostyEllipsedTextBox : TextBox
    {
        static FrostyEllipsedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyEllipsedTextBox), new FrameworkPropertyMetadata(typeof(FrostyEllipsedTextBox)));
        }
    }
}
