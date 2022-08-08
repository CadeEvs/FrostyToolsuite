using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LevelEditorPlugin.Render.Proxies
{
    public class WidgetProxy
    {
        public virtual Entities.Entity Entity => null;
        public Rect Bounds => new Rect(position, size);
        public bool IsVisible => visible;
        public bool DrawDebugOutlines
        {
            get => drawDebugOutlines;
            set => drawDebugOutlines = value;
        }

        protected Point position;
        protected Size size;
        protected Transform transform;
        protected float alpha;
        protected Color color;
        protected bool visible;
        protected bool drawDebugOutlines;
        protected ScaleTransform mirrorScale;

        protected Color debugColor;
        private Pen debugPen;

        protected Brush colorBrush;

        public WidgetProxy()
        {
            visible = true;
        }

        public virtual void Update()
        {
        }

        public virtual void Render(DrawingContext drawingContext)
        {
            if (debugPen == null)
            {
                debugPen = new Pen(new SolidColorBrush(debugColor), 1);
            }

            if (drawDebugOutlines)
            {
                drawingContext.PushTransform(new TranslateTransform(position.X + size.Width * 0.5, position.Y + size.Height * 0.5));
                drawingContext.PushTransform(transform);
                drawingContext.PushTransform(new TranslateTransform(-position.X + -size.Width * 0.5, -position.Y + -size.Height * 0.5));

                //drawingContext.DrawText(new FormattedText($"{transform.Value.M11}, {transform.Value.M22}", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 14.0, Brushes.White, 96.0), new Point(position.X, position.Y - 18));
                drawingContext.DrawRectangle(null, debugPen, new Rect(position, size));

                drawingContext.Pop();
                drawingContext.Pop();
                drawingContext.Pop();
            }
        }

        protected MatrixTransform MakeMatrixTransform(FrostySdk.Ebx.LinearTransform lt)
        {
            return new MatrixTransform(lt.right.x, lt.right.y, lt.up.x, lt.up.y, lt.trans.x, lt.trans.y);
        }

        protected MatrixTransform MakeMatrixTransform(SharpDX.Matrix m)
        {
            return new MatrixTransform(m.M11, m.M12, m.M21, m.M22, m.M41, m.M42);
        }

        protected Color MakeColor(FrostySdk.Ebx.Vec3 v)
        {
            return Color.FromArgb(0xff, (byte)(v.x * 255.0f), (byte)(v.y * 255.0f), (byte)(v.z * 255.0f));
        }

        protected Color MakeColor(FrostySdk.Ebx.UIElementColor c)
        {
            Color outColor = MakeColor(c.Rgb);
            outColor.A = (byte)(c.Alpha * 255.0f);
            return outColor;
        }

        protected HorizontalAlignment MakeHorizontalAlignment(FrostySdk.Ebx.UIElementAlignment align)
        {
            switch (align)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Left: return HorizontalAlignment.Left;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Right: return HorizontalAlignment.Right;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: return HorizontalAlignment.Center;
            }
            return HorizontalAlignment.Left;
        }

        protected VerticalAlignment MakeVerticalAlignment(FrostySdk.Ebx.UIElementAlignment align)
        {
            switch (align)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Top: return VerticalAlignment.Top;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: return VerticalAlignment.Center;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Bottom: return VerticalAlignment.Bottom;
            }
            return VerticalAlignment.Top;
        }
    }
}
