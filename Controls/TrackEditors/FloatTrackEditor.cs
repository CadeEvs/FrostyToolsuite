using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FrostySdk.Ebx;
using FloatTrack = LevelEditorPlugin.Entities.FloatTrack;
using TimelineEntity = LevelEditorPlugin.Entities.TimelineEntity;

namespace LevelEditorPlugin.Controls.TrackEditors
{
    public class FloatTrackEditor : TrackEditorBase
    {
        protected float minValue;
        protected float maxValue;

        protected Brush keyFrameBrush;
        protected Pen trackLinePen;

        public FloatTrackEditor()
        {
            keyFrameBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x5f, 0xd9, 0x5f));
            trackLinePen = new Pen(keyFrameBrush, 1);

            keyFrameBrush.Freeze();
            trackLinePen.Freeze();
        }

        protected override void InitializeData()
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;

            FloatTrack trackData = DataContext as Entities.FloatTrack;
            CurveData curveData = trackData.Data.CurveData.GetObjectAs<FrostySdk.Ebx.CurveData>();

            if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_One || curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Zero)
            {
                minValue = 0;
                maxValue = 1;
            }
            else if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Constant)
            {
                minValue = curveData.Value[0] - (curveData.Value[0] * 0.5f);
                maxValue = curveData.Value[0] + (curveData.Value[0] * 0.5f);
            }
            else
            {
                for (int i = 0; i < curveData.Value.Count; i++)
                {
                    float value = curveData.Value[i];

                    maxValue = (value > maxValue) ? value : maxValue;
                    minValue = (value < minValue) ? value : minValue;

                    if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Complex)
                    {
                        float itx = curveData.InTanX[i];
                        float ity = curveData.InTanY[i];
                        float otx = curveData.OutTanX[i];
                        float oty = curveData.OutTanY[i];

                        maxValue = (itx > maxValue) ? itx : maxValue;
                        maxValue = (ity > maxValue) ? ity : maxValue;
                        maxValue = (otx > maxValue) ? otx : maxValue;
                        maxValue = (oty > maxValue) ? oty : maxValue;

                        minValue = (itx < minValue) ? itx : minValue;
                        minValue = (ity < minValue) ? ity : minValue;
                        minValue = (otx < minValue) ? otx : minValue;
                        minValue = (oty < minValue) ? oty : minValue;

                    }
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            FloatTrack trackData = DataContext as Entities.FloatTrack;
            TimelineEntity timeline = trackData.Timeline;
            if (timeline == null)
                return;

            float startTime = timeline.Data.StartTime;
            float endTime = timeline.Data.EndTime;

            double height = ActualHeight;
            double width = ActualWidth;

            if (double.IsInfinity(width) || double.IsNaN(width) || double.IsInfinity(height) || double.IsNaN(height))
                return;

            FrostySdk.Ebx.CurveData curveData = trackData.Data.CurveData.GetObjectAs<FrostySdk.Ebx.CurveData>();
            if (curveData == null)
                return;

            Pen linePen = new Pen(Brushes.Black, 0.5);

            drawingContext.DrawLine(linePen, new Point(0, height), new Point(width, height));
            drawingContext.PushClip(new RectangleGeometry(new Rect(1, 1, width - 2, height - 2)));

            List<Point> points = new List<Point>();
            if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Zero || curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_One || curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Constant)
            {
                double y = height - 10;
                if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_One)
                {
                    y = 0;
                }
                else if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Constant)
                {
                    y = (height - 10) * 0.5;
                }

                Point a = new Point(0, y);
                Point b = new Point(width - 10, y);
                Size size = new Size(10, 10);

                drawingContext.DrawRectangle(keyFrameBrush, null, new Rect(a, size));
                drawingContext.DrawRectangle(keyFrameBrush, null, new Rect(b, size));

                points.Add(new Point(a.X + 5, a.Y + 5));
                points.Add(new Point(b.X + 5, b.Y + 5));
            }
            else
            {
                for (int i = 0; i < curveData.Time.Count; i++)
                {
                    float currentTime = curveData.Time[i];
                    float value = curveData.Value[i];

                    double x = ((currentTime - startTime) / (endTime - startTime)) * (width - 10);
                    double y = (1.0 - ((value - minValue) / (maxValue - minValue))) * (height - 10);

                    drawingContext.DrawRectangle(keyFrameBrush, null, new Rect(x, y, 10, 10));
                    points.Add(new Point(x + 5, y + 5));
                }
            }

            if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Basic_Linear || curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_One || curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Zero || curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Constant)
            {
                for (int i = 1; i < points.Count; i++)
                {
                    drawingContext.DrawLine(trackLinePen, points[i - 1], points[i]);
                }
            }
            else if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Complex)
            {
                PathGeometry lineGeometry = new PathGeometry();
                for (int i = 1; i < points.Count; i++)
                {
                    PathFigure lineFigure = new PathFigure();
                    lineFigure.StartPoint = points[i - 1];

                    double aox = curveData.OutTanX[i - 1];
                    double aoy = 1 - curveData.OutTanY[i - 1];

                    double bix = curveData.InTanX[i];
                    double biy = 1 - curveData.InTanY[i];

                    aox = Reposition(points[i - 1].X + (aox * 10), width);
                    aoy = Reposition(points[i - 1].Y + (aoy * 10), height, true);

                    bix = Reposition(points[i].X - (bix * 10), width);
                    biy = Reposition(points[i].Y - (biy * 10), height, true);

                    BezierSegment segment = new BezierSegment(new Point(aox + 5, aoy + 5), new Point(bix + 5, biy + 5), points[i], true);

                    lineFigure.Segments.Add(segment);
                    lineGeometry.Figures.Add(lineFigure);
                }

                drawingContext.DrawGeometry(null, trackLinePen, lineGeometry);
            }

            drawingContext.Pop();
        }

        private double Reposition(double value, double widthOrHeight, bool oneMinus = false)
        {
            ////double v = (oneMinus)
            ////    ? ((1 - (value - minValue) / (maxValue - minValue)))
            ////    : (value - minValue) / (maxValue - minValue);

            ////return v * (widthOrHeight - 10);
            return value;
        }
    }
}
