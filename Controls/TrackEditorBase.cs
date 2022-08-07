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
    public class TrackEditorBase : Control
    {
        //protected float minValue;
        //protected float maxValue;
        //protected FontData fontData;

        public TrackEditorBase()
        {
            //TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            //fontData = FontData.MakeFont(new Typeface("Consolas"), 10);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            //var timeline = Timeline as Entities.TimelineEntity;
            //if (timeline == null)
            //    return;

            //float startTime = timeline.Data.StartTime;
            //float endTime = timeline.Data.EndTime;

            //double height = ActualHeight;
            //double width = ActualWidth;

            //if (double.IsInfinity(width) || double.IsNaN(width) || double.IsInfinity(height) || double.IsNaN(height))
            //    return;

            //FrostySdk.Ebx.CurveData curveData = TrackData as FrostySdk.Ebx.CurveData;
            //if (curveData == null)
            //    return;

            //Pen linePen = new Pen(Brushes.Black, 0.5);
            //Brush lineColor = new SolidColorBrush(Color.FromArgb(0xff, 0x5f, 0xd9, 0x5f));

            //drawingContext.DrawLine(linePen, new Point(0, height), new Point(width, height));
            //drawingContext.PushClip(new RectangleGeometry(new Rect(1, 1, width - 2, height - 2)));

            //if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Zero || curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_One)
            //{
            //    double y = height - 10;
            //    if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_One)
            //    {
            //        y = 0;
            //    }

            //    Point a = new Point(0, y);
            //    Point b = new Point(width - 10, y);
            //    Size size = new Size(10, 10);

            //    drawingContext.DrawRectangle(lineColor, null, new Rect(a, size));
            //    drawingContext.DrawRectangle(lineColor, null, new Rect(b, size));

            //    drawingContext.DrawLine(new Pen(lineColor, 1), new Point(a.X + 5, a.Y + 5), new Point(b.X + 5, b.Y + 5));
            //}
            //else
            //{
            //    Point prevPos = new Point();
            //    for (int i = 0; i < curveData.Time.Count; i++)
            //    {
            //        float currentTime = curveData.Time[i];
            //        float value = curveData.Value[i];

            //        double x = ((currentTime - startTime) / (endTime - startTime)) * (width - 10);
            //        double y = (1.0 - ((value - minValue) / (maxValue - minValue))) * (height - 10);

            //        drawingContext.DrawRectangle(lineColor, null, new Rect(x, y, 10, 10));

            //        //var valueGlyphRun = ConvertTextLinesToGlyphRun(new Point(x + 12, y), value.ToString());
            //        //drawingContext.DrawGlyphRun(Brushes.White, valueGlyphRun);

            //        if (i > 0)
            //        {
            //            Point curPos = new Point(x + 5, y + 5);
            //            drawingContext.DrawLine(new Pen(lineColor, 1), prevPos, curPos);
            //        }

            //        prevPos = new Point(x + 5, y + 5);
            //    }
            //}

            //drawingContext.Pop();
        }

        protected virtual void InitializeData()
        {
        }

        //private void UpdateValues(FrostySdk.Ebx.CurveData newData)
        //{
        //    minValue = 0.0f;
        //    maxValue = 0.0f;

        //    if (newData == null)
        //        return;

        //    minValue = float.MaxValue;
        //    maxValue = float.MinValue;

        //    for (int i = 0; i < newData.Value.Count; i++)
        //    {
        //        minValue = (newData.Value[i] < minValue) ? newData.Value[i] : minValue;
        //        maxValue = (newData.Value[i] > maxValue) ? newData.Value[i] : maxValue;
        //    }

        //    InvalidateVisual();
        //}

        //private GlyphRun ConvertTextLinesToGlyphRun(Point position, string line)
        //{
        //    var glyphIndices = new List<ushort>();
        //    var advanceWidths = new List<double>();
        //    var glyphOffsets = new List<Point>();

        //    var y = -position.Y;
        //    var x = position.X;

        //    for (int j = 0; j < line.Length; ++j)
        //    {
        //        var glyphIndex = fontData.GlyphTypeface.CharacterToGlyphMap[line[j]];
        //        glyphIndices.Add(glyphIndex);
        //        advanceWidths.Add(0);
        //        glyphOffsets.Add(new Point(x, y));

        //        x += fontData.AdvanceWidth;
        //    }

        //    return new GlyphRun(
        //        fontData.GlyphTypeface,
        //        0,
        //        false,
        //        fontData.RenderingEmSize,
        //        96.0f,
        //        glyphIndices,
        //        fontData.BaselineOrigin,
        //        advanceWidths,
        //        glyphOffsets,
        //        null,
        //        null,
        //        null,
        //        null,
        //        null);
        //}

        //private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    var editor = o as TrackEditorBase;
        //    editor.UpdateValues(e.NewValue as FrostySdk.Ebx.CurveData);
        //}

        protected override void OnInitialized(EventArgs e)
        {
            InitializeData();
        }
    }
}
