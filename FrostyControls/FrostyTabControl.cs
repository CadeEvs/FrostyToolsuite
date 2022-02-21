using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Frosty.Controls
{
    [TemplatePart(Name = PART_ScrollLeft, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_ScrollRight, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
    public class FrostyTabControl : TabControl
    {
        private const string PART_ScrollLeft = "PART_ScrollLeft";
        private const string PART_ScrollRight = "PART_ScrollRight";
        private const string PART_ScrollViewer = "PART_ScrollViewer";

        private RepeatButton scrollLeftButton;
        private RepeatButton scrollRightButton;
        private ScrollViewer scrollViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            scrollLeftButton = GetTemplateChild(PART_ScrollLeft) as RepeatButton;
            scrollLeftButton.Click += ScrollLeftButton_Click;
            scrollRightButton = GetTemplateChild(PART_ScrollRight) as RepeatButton;
            scrollRightButton.Click += ScrollRightButton_Click;

            scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
            scrollViewer.Loaded += (s, e) => UpdateControls();
            scrollViewer.ScrollChanged += (s, e) => UpdateControls();

            SelectionChanged += (s, e) => ScrollToSelectedItem();
        }

        private void ScrollLeftButton_Click(object sender, RoutedEventArgs e)
        {
            double leftItemOffset = Math.Max(scrollViewer.HorizontalOffset, 0);
            TabItem leftItem = GetItemByOffset(leftItemOffset);

            ScrollToItem(leftItem);
        }

        private void ScrollRightButton_Click(object sender, RoutedEventArgs e)
        {
            double rightItemOffset = Math.Min(scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth + 1, scrollViewer.ExtentWidth);
            TabItem rightItem = GetItemByOffset(rightItemOffset);

            ScrollToItem(rightItem);
        }

        private void UpdateControls()
        {
            double horizOffset = Math.Max(scrollViewer.HorizontalOffset, 0);
            double srcWidth = Math.Max(scrollViewer.ScrollableWidth, 0);

            scrollLeftButton.Visibility = (srcWidth == 0) ? Visibility.Collapsed : Visibility.Visible;
            scrollRightButton.Visibility = (srcWidth == 0) ? Visibility.Collapsed : Visibility.Visible;
            scrollLeftButton.IsEnabled = (horizOffset > 0);
            scrollRightButton.IsEnabled = (horizOffset < srcWidth);
        }

        private void ScrollToSelectedItem()
        {
            if (!(SelectedItem is TabItem ti))
                return;

            if(ti.ActualWidth == 0.0 && !ti.IsLoaded)
            {
                ti.Loaded += (s, e) => ScrollToSelectedItem();
                return;
            }
            ScrollToItem(ti);
        }

        private void ScrollToItem(TabItem ti)
        {
            double leftItemsWidth = 0.0;
            foreach(TabItem si in Items)
            {
                if (si == ti)
                    break;
                leftItemsWidth += si.ActualWidth;
            }

            if (leftItemsWidth + ti.ActualWidth > scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth)
            {
                double currentHorizontalOffset = (leftItemsWidth + ti.ActualWidth) - scrollViewer.ViewportWidth;
                scrollViewer.ScrollToHorizontalOffset(currentHorizontalOffset);
            }
            else if (leftItemsWidth < scrollViewer.HorizontalOffset)
            {
                double currentHorizontalOffset = leftItemsWidth;
                scrollViewer.ScrollToHorizontalOffset(currentHorizontalOffset);
            }
        }

        private TabItem GetItemByOffset(double offset)
        {
            double currentItemsWidth = 0;
            foreach (TabItem ti in Items)
            {
                if (currentItemsWidth + ti.ActualWidth >= offset)
                    return ti;

                currentItemsWidth += ti.ActualWidth;
            }

            return Items[Items.Count - 1] as TabItem;
        }
    }
}
