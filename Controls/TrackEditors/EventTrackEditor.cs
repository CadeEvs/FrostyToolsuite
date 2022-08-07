using LevelEditorPlugin.Editors;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls.TrackEditors
{
    public class EventTrackEditor : TrackEditorBase
    {
        protected Brush eventKeyframeBrush;
        protected Geometry eventKeyframeGeometry;

        public EventTrackEditor()
        {
            eventKeyframeBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xbf, 0x73));
        }

        protected override void InitializeData()
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var trackData = DataContext as Entities.EventTrack;
            var timeline = trackData.Timeline;
            if (timeline == null)
                return;

            float startTime = timeline.Data.StartTime;
            float endTime = timeline.Data.EndTime;

            double height = ActualHeight;
            double width = ActualWidth;

            if (double.IsInfinity(width) || double.IsNaN(width) || double.IsInfinity(height) || double.IsNaN(height))
                return;

            Pen linePen = new Pen(Brushes.Black, 0.5);
            drawingContext.DrawLine(linePen, new Point(0, height), new Point(width, height));
            drawingContext.PushClip(new RectangleGeometry(new Rect(1, 1, width - 2, height - 2)));

            for (int i = 0; i < trackData.Data.Keyframes.Count; i++)
            {
                float time = trackData.Data.Keyframes[i].Time;
                double x = ((time - startTime) / (endTime - startTime)) * (width - 10);

                drawingContext.DrawRectangle(eventKeyframeBrush, null, new Rect(x, 0, 10, ActualHeight));
            }

            drawingContext.Pop();
        }
    }
}