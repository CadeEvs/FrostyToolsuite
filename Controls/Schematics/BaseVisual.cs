using System;
using System.Windows;
using System.Windows.Input;

namespace LevelEditorPlugin.Controls
{
    public abstract class BaseVisual
    {
        public virtual object Data => null;

        public Guid UniqueId;
        public Rect Rect;
        public bool IsSelected;

        public BaseVisual(double x, double y)
        {
            Rect.X = x;
            Rect.Y = y;
        }

        public virtual bool OnMouseOver(Point mousePos)
        {
            return false;
        }

        public virtual bool OnMouseDown(Point mousePos, MouseButton mouseButton)
        {
            return false;
        }

        public virtual bool OnMouseUp(Point mousePos, MouseButton mouseButton)
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