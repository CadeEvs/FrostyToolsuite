using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls
{
    public abstract class BaseVisual
    {
        public virtual object Data => null;

        public Guid UniqueId;
        public Rect Rect;
        public Rect Bounds => new Rect(Rect.X - 6, Rect.Y, Rect.Width + 12, Rect.Height);
        public bool IsSelected;

        public BaseVisual(double inX, double inY)
        {
            Rect.X = inX;
            Rect.Y = inY;
        }

        public virtual bool OnMouseOver(Point mousePos)
        {
            return false;
        }

        public virtual bool OnMouseDown(Point mousePos, MouseButton mouseButton)
        {
            return false;
        }

        public virtual bool OnMouseUp(Point mousePos, MouseButton mouseButton, bool hasMouseMoved)
        {
            return false;
        }

        public virtual bool OnMouseLeave()
        {
            return false;
        }

        public virtual bool HitTest(Point mousePos)
        {
            return true;
        }

        public virtual void Move(Point newPos)
        {
            Rect.Location = newPos;
        }

        public abstract void Update();
        public abstract void Render(SchematicsCanvas.DrawingContextState state);
        public virtual void RenderDebug(SchematicsCanvas.DrawingContextState state)
        {
            // do nothing
        }

        public void DrawSpecificRoundedRectangle(DrawingContext dc, Brush brush, Pen pen, Rect rect, CornerRadius cornerRadius)
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