using System.Windows;
using System.Windows.Controls;

namespace Frosty.Controls
{
    public class FrostySpinner : Control
    {
        static FrostySpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostySpinner), new FrameworkPropertyMetadata(typeof(FrostySpinner)));
        }
    }
}
