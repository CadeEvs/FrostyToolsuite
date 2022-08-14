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
    }
}