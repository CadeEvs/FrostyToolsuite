using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Frosty.Controls
{
    public class FrostyDockablePanel : ContentControl
    {
        #region -- Properties --

        #region -- Title --
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(FrostyDockablePanel), new FrameworkPropertyMetadata(""));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        #endregion

        #region -- Icon --
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(FrostyDockablePanel), new FrameworkPropertyMetadata(null));
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        #endregion

        #region -- TitleBarVisible --
        public static readonly DependencyProperty TitleBarVisibleProperty = DependencyProperty.Register("TitleBarVisible", typeof(bool), typeof(FrostyDockablePanel), new FrameworkPropertyMetadata(true));
        public bool TitleBarVisible
        {
            get => (bool)GetValue(TitleBarVisibleProperty);
            set => SetValue(TitleBarVisibleProperty, value);
        }
        #endregion

        #region -- BorderVisible --
        public static readonly DependencyProperty BorderVisibleProperty = DependencyProperty.Register("BorderVisible", typeof(bool), typeof(FrostyDockablePanel), new FrameworkPropertyMetadata(true));
        public bool BorderVisible
        {
            get => (bool)GetValue(BorderVisibleProperty);
            set => SetValue(BorderVisibleProperty, value);
        }
        #endregion

        #region -- CloseButtonVisible --
        public static readonly DependencyProperty CloseButtonVisibleProperty = DependencyProperty.Register("CloseButtonVisible", typeof(bool), typeof(FrostyDockablePanel), new FrameworkPropertyMetadata(false));
        public bool CloseButtonVisible
        {
            get => (bool)GetValue(CloseButtonVisibleProperty);
            set => SetValue(CloseButtonVisibleProperty, value);
        }
        #endregion

        #endregion

        static FrostyDockablePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyDockablePanel), new FrameworkPropertyMetadata(typeof(FrostyDockablePanel)));
        }
    }
}
