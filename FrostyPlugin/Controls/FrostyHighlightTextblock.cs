using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Frosty.Core.Controls
{
    public class FrostyHightlightTextBlock : TextBlock
    {
        public FrostyHightlightTextBlock()
        {
        }

        public string HighlightText
        {
            get => (string)GetValue(HighlightTextProperty);
            set => SetValue(HighlightTextProperty, value);
        }

        private static readonly SolidColorBrush HighlightBackgroundColor = new SolidColorBrush(Color.FromArgb(0xff, 12, 60, 98));
        private static readonly SolidColorBrush HighlightForegroundColor = new SolidColorBrush(Color.FromArgb(0xff, 149, 197, 235));

        private static void OnDataChanged(DependencyObject source,
                                          DependencyPropertyChangedEventArgs e)
        {
            TextBlock tb = (TextBlock)source;

            if (tb.Text.Length == 0)
                return;

            string textUpper = tb.Text.ToUpper();
            string toFind = ((string)e.NewValue).ToUpper();
            int firstIndex = textUpper.IndexOf(toFind);

            if (firstIndex != -1)
            {
                string firstStr = tb.Text.Substring(0, firstIndex);
                string foundStr = tb.Text.Substring(firstIndex, toFind.Length);
                string endStr = tb.Text.Substring(firstIndex + toFind.Length,
                                                 tb.Text.Length - (firstIndex + toFind.Length));

                tb.Inlines.Clear();
                var run = new Run {Text = firstStr};
                tb.Inlines.Add(run);
                run = new Run
                {
                    Background = HighlightBackgroundColor,
                    Foreground = HighlightForegroundColor,
                    Text = foundStr
                };
                tb.Inlines.Add(run);
                run = new Run {Text = endStr};
                tb.Inlines.Add(run);
            }
            else
            {
                string tbText = tb.Text;

                tb.Inlines.Clear();
                var run = new Run {Text = tbText};
                tb.Inlines.Add(run);
            }
        }

        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.Register("HighlightText",
                                        typeof(string),
                                        typeof(FrostyHightlightTextBlock),
                                        new FrameworkPropertyMetadata(null, OnDataChanged));
    }
}
