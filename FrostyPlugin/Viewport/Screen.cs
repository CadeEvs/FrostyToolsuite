using System;

namespace Frosty.Core.Viewport
{
    [Flags]
    public enum MouseButton
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 4
    }
    public abstract class Screen : IDisposable
    {
        public IViewport Viewport { get; private set; }

        public Screen()
        {
        }

        public void SetViewport(IViewport inViewport)
        {
            Viewport = inViewport;
        }

        public virtual void Dispose()
        {

        }

        public abstract void Update(double timestep);

        public abstract void Render();

        public virtual void CreateSizeDependentBuffers()
        {
        }

        public virtual void CreateBuffers()
        {
        }

        public virtual void DisposeSizeDependentBuffers()
        {
        }

        public virtual void DisposeBuffers()
        {
        }

        public virtual void Opened()
        {

        }

        public virtual void Closed()
        {

        }

        public virtual void MouseDown(int x, int y, MouseButton button)
        {

        }

        public virtual void MouseMove(int x, int y)
        {

        }

        public virtual void MouseUp(int x, int y, MouseButton button)
        {

        }

        public virtual void MouseLeave()
        {

        }

        public virtual void MouseScroll(int delta)
        {

        }

        public virtual void KeyDown(int key)
        {

        }

        public virtual void KeyUp(int key)
        {

        }

        public virtual void CharTyped(char ch)
        {

        }
    }
}
