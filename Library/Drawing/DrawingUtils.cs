using System.Windows;
using System.Windows.Media;
using static LevelEditorPlugin.Controls.SchematicsCanvas;

namespace LevelEditorPlugin.Library.Drawing
{
    public static class DrawingUtils
    {
        public static void DrawRoundedRectangle(this DrawingContext dc, Brush brush, Pen pen, Rect rect, CornerRadius cornerRadius)
        {
            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                bool isStroked = pen != null;
                const bool isSmoothJoin = true;

                context.BeginFigure(rect.TopLeft + new Vector(0, cornerRadius.TopLeft), brush != null, true);
                context.ArcTo(new Point(rect.TopLeft.X + cornerRadius.TopLeft, rect.TopLeft.Y),
                    new Size(cornerRadius.TopLeft, cornerRadius.TopLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.TopRight - new Vector(cornerRadius.TopRight, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.TopRight.X, rect.TopRight.Y + cornerRadius.TopRight),
                    new Size(cornerRadius.TopRight, cornerRadius.TopRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomRight - new Vector(0, cornerRadius.BottomRight), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomRight.X - cornerRadius.BottomRight, rect.BottomRight.Y),
                    new Size(cornerRadius.BottomRight, cornerRadius.BottomRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomLeft + new Vector(cornerRadius.BottomLeft, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - cornerRadius.BottomLeft),
                    new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.Close();
            }
            geometry.Freeze();
            dc.DrawGeometry(brush, pen, geometry);
        }
        public static void DrawCurvedLine(this DrawingContextState state, Pen pen, Point a, Point b, double curve = 30.0d)
        {
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;
            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(a, false, false);
                context.LineTo(a, true, false);
                Vector c, d;
                c = new Vector(a.X + (curve * state.Scale), a.Y);
                d = new Vector(b.X - (curve * state.Scale), b.Y);
                context.BezierTo(
                    new Point(c.X, c.Y),
                    new Point(d.X, d.Y),
                    new Point(b.X, b.Y),
                    true, false);
                context.LineTo(b, true, false);
            }
            geometry.Freeze();
            state.DrawingContext.DrawGeometry(null, pen, geometry);
        }
    }
}