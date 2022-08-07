using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls
{
    public class TrackScrubberControl : Control
    {
        public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(float), typeof(TrackScrubberControl), new FrameworkPropertyMetadata(0.0f, OnCurrentTimeChanged));
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(float), typeof(TrackScrubberControl), new FrameworkPropertyMetadata(0.0f));
        public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register("EndTime", typeof(float), typeof(TrackScrubberControl), new FrameworkPropertyMetadata(0.0f));

        public float CurrentTime
        {
            get => (float)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public float StartTime
        {
            get => (float)GetValue(StartTimeProperty);
            set => SetValue(StartTimeProperty, value);
        }
        public float EndTime
        {
            get => (float)GetValue(EndTimeProperty);
            set => SetValue(EndTimeProperty, value);
        }

        private LineGeometry playbackLine;
        private Pen playbackLinePen;

        public TrackScrubberControl()
        {
            IsHitTestVisible = false;

            playbackLinePen = new Pen(Brushes.Orange, 1);
            playbackLinePen.Freeze();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            playbackLine = new LineGeometry(new Point(0, 0), new Point(0, arrangeBounds.Height));
            playbackLine.Transform = new TranslateTransform(9, 0);

            return base.ArrangeOverride(arrangeBounds);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawGeometry(null, playbackLinePen, playbackLine);
        }

        private static void OnCurrentTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = o as TrackScrubberControl;
            double width = ctrl.ActualWidth - 18;

            double currentPos = ((ctrl.CurrentTime - ctrl.StartTime) / (ctrl.EndTime - ctrl.StartTime)) * width;
            var transform = ctrl.playbackLine.Transform as TranslateTransform;

            transform.X = currentPos + 9;
        }
    }
}
