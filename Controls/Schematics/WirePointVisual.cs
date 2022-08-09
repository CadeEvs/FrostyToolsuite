using System.Windows;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls
{
    public class WirePointVisual : BaseVisual
    {
        public WireVisual Wire;

        public WirePointVisual(WireVisual wire, double x, double y)
            : base(x - 5, y - 5)
        {
            Wire = wire;
            Rect.Width = 10;
            Rect.Height = 10;
        }

        public override void Update()
        {
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            Pen wirePen = null;
            switch (Wire.WireType)
            {
                case 0: wirePen = state.WireLinkPen; break;
                case 1: wirePen = state.WireEventPen; break;
                case 2: wirePen = state.WirePropertyPen; break;
            }

            state.DrawingContext.DrawRectangle((IsSelected) ? state.NodeSelectedBrush : wirePen.Brush, null, new Rect(state.WorldMatrix.Transform(new Point(Rect.X, Rect.Y)), new Size(Rect.Width * state.Scale, Rect.Height * state.Scale)));
        }
    }
}