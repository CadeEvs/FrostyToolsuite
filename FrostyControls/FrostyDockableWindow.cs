using System.Windows;

namespace Frosty.Controls
{
    public class FrostyDockableWindow : FrostyWindow
    {
        public DependencyObject WindowParent { get; set; }

        static FrostyDockableWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyDockableWindow), new FrameworkPropertyMetadata(typeof(FrostyDockableWindow)));
        }

        public FrostyDockableWindow()
        {
        }
    }
}
