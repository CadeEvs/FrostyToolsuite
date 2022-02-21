using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Frosty.Controls
{
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_DragLabel, Type = typeof(Label))]
    public class FrostyTabItem : TabItem
    {
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_DragLabel = "PART_DragLabel";

        #region -- Properties --

        #region -- Icon --

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(FrostyTabItem), new FrameworkPropertyMetadata(null));
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

        #region -- SvgIcon --

        public static readonly DependencyProperty SvgIconProperty = DependencyProperty.Register("SvgIcon", typeof(Geometry), typeof(FrostyTabItem), new FrameworkPropertyMetadata(null));
        public Geometry SvgIcon
        {
            get => (Geometry)GetValue(SvgIconProperty);
            set => SetValue(SvgIconProperty, value);
        }

        #endregion

        #region -- CloseButtonVisibile --

        public static readonly DependencyProperty CloseButtonVisibleProperty = DependencyProperty.Register("CloseButtonVisible", typeof(bool), typeof(FrostyTabItem), new FrameworkPropertyMetadata(false));
        public bool CloseButtonVisible
        {
            get => (bool)GetValue(CloseButtonVisibleProperty);
            set => SetValue(CloseButtonVisibleProperty, value);
        }

        #endregion

        #endregion

        public string TabId { get; set; }

        private ButtonBase closeButton;
        private Label dragLabel;

        private event RoutedEventHandler closeButtonClick;
        public event RoutedEventHandler CloseButtonClick
        {
            add => closeButtonClick += value;
            remove => closeButtonClick -= value;
        }

        private event MouseEventHandler middleMouseButtonClick;
        public event MouseEventHandler MiddleMouseButtonClick
        {
            add => middleMouseButtonClick += value;
            remove => middleMouseButtonClick -= value;
        }

        private event MouseEventHandler rightMouseButtonClick;
        public event MouseEventHandler RightMouseButtonClick
        {
            add => rightMouseButtonClick += value;
            remove => rightMouseButtonClick -= value;
        }

        static FrostyTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyTabItem), new FrameworkPropertyMetadata(typeof(FrostyTabItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            closeButton = GetTemplateChild(PART_CloseButton) as ButtonBase;
            dragLabel = GetTemplateChild(PART_DragLabel) as Label;

            if (closeButton != null && closeButtonClick != null)
                closeButton.Click += closeButtonClick;
            if(dragLabel != null)
                dragLabel.MouseUp += (s, o) =>
                {
                    if (o.ChangedButton == MouseButton.Right)
                        rightMouseButtonClick?.Invoke(s, o);
                    else if (o.ChangedButton == MouseButton.Middle)
                        middleMouseButtonClick?.Invoke(s, o);
                };
        }
    }
}
