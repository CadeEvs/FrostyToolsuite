using System.Windows;
using System.Windows.Media;

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
    }
}