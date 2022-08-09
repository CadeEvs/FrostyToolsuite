using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditorPlugin.Data;
using LevelEditorPlugin.Managers;

namespace LevelEditorPlugin.Controls
{
public class InterfaceShortcutNodeVisual : InterfaceNodeVisual
    {
        public override object Data => ShortcutData;

        public BaseNodeVisual OwnerNode;
        public Port OwnerPort;

        public IShortcutNodeData ShortcutData;

        public InterfaceShortcutNodeVisual(BaseNodeVisual node, Port port, IShortcutNodeData inData, int direction, double x, double y)
            : base(Guid.Empty, null, direction, x, y)
        {
            ShortcutData = inData;

            OwnerNode = node;
            OwnerPort = port;
            Rect.Size = node.Rect.Size;
            FieldType = port.PortType;
            Self = new Port(this) { Name = port.Name, NameHash = port.NameHash };
            GlyphWidth = node.GlyphWidth;

            icon = Geometry.Parse("M 8.888587 17.982632 C 8.0968795 17.955544 7.2488877 17.851374 6.5626389 17.696906 5.4000834 17.435226 4.5358167 17.0054 4.111736 16.477999 3.994131 16.331741 3.8612254 16.07883 3.8176485 15.918367 c -0.014632 -0.05388 -0.030536 -0.189796 -0.035343 -0.30204 -0.010268 -0.239767 0.021885 -0.409145 0.1166632 -0.614558 0.3388827 -0.734465 1.4424479 -1.361551 2.9108137 -1.654029 0.1748641 -0.03483 0.4133152 -0.07564 0.5298913 -0.09068 0.1165761 -0.01504 0.239213 -0.0322 0.2725265 -0.03812 l 0.06057 -0.01077 -1.2804389 -2 C 5.0099783 9.048977 4.9527379 8.9517267 4.7871107 8.4809246 4.646399 8.0809451 4.5650893 7.7172318 4.5254051 7.3102638 4.4988012 7.0374385 4.5154273 6.4394949 4.5570707 6.1714286 4.6861372 5.3406039 5.0123269 4.5894207 5.5389725 3.9102041 5.680467 3.7277182 6.0293892 3.36546 6.2145958 3.2087582 7.2282394 2.351122 8.5129689 1.929703 9.8470992 2.0172189 11.380677 2.117818 12.800316 2.9236621 13.656509 4.1795918 14.21005 4.991569 14.489173 5.8946364 14.489111 6.8733693 14.489062 7.6314134 14.327698 8.3202909 13.99383 8.9877551 13.919515 9.1363249 13.463632 9.8902867 12.769721 11.012245 12.16159 11.99551 11.183455 13.578776 10.596089 14.530612 10.008723 15.482449 9.5195287 16.2706 9.5089917 16.282058 9.493955 16.29841 9.3795546 16.12678 8.9772536 15.484314 L 8.4646739 14.665737 8.326087 14.676657 c -0.9899466 0.078 -2.1502448 0.37234 -2.7068462 0.686655 -0.1005755 0.05679 -0.2768495 0.181239 -0.2768495 0.195446 0 0.01702 0.1999966 0.157469 0.3097826 0.217545 0.5912757 0.323552 1.6397459 0.569981 2.8369565 0.66679 0.4173097 0.03374 1.5137986 0.03877 1.9076086 0.0087 1.389354 -0.105943 2.526354 -0.386235 3.089906 -0.76172 0.192881 -0.128513 0.191071 -0.123087 0.06989 -0.209582 -0.343693 -0.245309 -1.131971 -0.523651 -1.907611 -0.673579 l -0.141841 -0.02742 0.05349 -0.08364 c 0.02942 -0.046 0.224741 -0.351806 0.43405 -0.679561 0.209309 -0.327756 0.382157 -0.598168 0.384106 -0.600917 0.0059 -0.0084 0.458092 0.108237 0.691931 0.178451 0.12106 0.03635 0.322826 0.10675 0.44837 0.156444 0.937499 0.371096 1.486182 0.85133 1.657905 1.451079 0.03105 0.10844 0.03689 0.167309 0.03631 0.365963 -6.15e-4 0.210595 -0.0056 0.252959 -0.04554 0.383673 -0.154562 0.506425 -0.556884 0.90621 -1.257373 1.249447 -1.159089 0.567949 -2.982414 0.851942 -5.021739 0.782165 z M 9.8660693 8.373544 C 10.602557 8.2571513 11.246517 7.7302759 11.509928 7.0285714 11.602003 6.7832922 11.626179 6.634566 11.625373 6.3183673 11.624672 6.0434354 11.621371 6.0120831 11.574148 5.8320408 11.516907 5.6138046 11.404852 5.3656194 11.277041 5.1739899 11.165992 5.0074934 10.881334 4.7242078 10.714674 4.614336 9.9196521 4.0902103 8.9126518 4.1253055 8.1700154 4.7030206 7.7776821 5.0082263 7.5098685 5.4384856 7.4033639 5.9346939 c -0.041319 0.1925089 -0.041319 0.5911645 0 0.7836734 0.137521 0.6407156 0.5400483 1.1616998 1.1265274 1.4580454 0.4119564 0.20816 0.8538602 0.2733556 1.336178 0.1971313 z");

            Update();
        }

        public override SchematicsLayout.Node GenerateLayout()
        {
            return new SchematicsLayout.Node();
        }

        public override void Update()
        {
            if (ShortcutData.DisplayName != Self.Name)
            {
                Self.Name = ShortcutData.DisplayName;
                Update();

                foreach (InterfaceShortcutNodeVisual child in OwnerPort.ShortcutChildren)
                {
                    child.ShortcutData.DisplayName = Self.Name;
                    child.Update();
                }
            }

            base.Update();
        }

        public override bool OnMouseOver(Point mousePos)
        {
            Rect portRect = new Rect(Rect.X + Self.Rect.X, Rect.Y + Self.Rect.Y, Self.Rect.Width, Self.Rect.Height);
            bool changedHighlight = false;

            if (portRect.Contains(mousePos))
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                HightlightedPort = Self;
                Self.IsHighlighted = true;
                changedHighlight = true;
            }
            else if (Self.IsHighlighted)
            {
                HightlightedPort = null;
                Self.IsHighlighted = false;
                changedHighlight = true;
            }

            return changedHighlight;
        }

        public override bool OnMouseLeave()
        {
            if (HightlightedPort != null)
            {
                HightlightedPort.IsHighlighted = false;
                HightlightedPort = null;
                return true;
            }
            return false;
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            base.Render(state);

            if (IsSelected)
            {
                if (this == OwnerPort.ShortcutNode)
                {
                    double targetX = (Direction == 0) ? Rect.X : Rect.X + Rect.Width;
                    for (int i = 0; i < OwnerPort.ShortcutChildren.Count; i++)
                    {
                        InterfaceShortcutNodeVisual childNode = OwnerPort.ShortcutChildren[i];
                        double sourceX = (childNode.Direction == 0) ? childNode.Rect.X : childNode.Rect.X + childNode.Rect.Width;

                        state.DrawingContext.DrawLine(state.ShortcutWirePen,
                            state.WorldMatrix.Transform(new Point(sourceX, childNode.Rect.Y + childNode.Self.Rect.Y + 6)),
                            state.WorldMatrix.Transform(new Point(targetX, Rect.Y + Self.Rect.Y + 6)));

                        if (state.InvScale < 2.5)
                        {
                            state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                                state.WorldMatrix.Transform(new Point(sourceX - 5, childNode.Rect.Y + childNode.Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                                ));
                            state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                                state.WorldMatrix.Transform(new Point(targetX - 5, Rect.Y + Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                                ));
                        }
                    }
                }
                else
                {
                    double sourceX = (OwnerPort.ShortcutNode.Direction == 0) ? OwnerPort.ShortcutNode.Rect.X : OwnerPort.ShortcutNode.Rect.X + OwnerPort.ShortcutNode.Rect.Width;
                    double targetX = (Direction == 0) ? Rect.X : Rect.X + Rect.Width;

                    state.DrawingContext.DrawLine(state.ShortcutWirePen,
                        state.WorldMatrix.Transform(new Point(sourceX, OwnerPort.ShortcutNode.Rect.Y + OwnerPort.ShortcutNode.Self.Rect.Y + 6)),
                        state.WorldMatrix.Transform(new Point(targetX, Rect.Y + Self.Rect.Y + 6))
                        );

                    if (state.InvScale < 2.5)
                    {
                        state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                            state.WorldMatrix.Transform(new Point(sourceX - 5, OwnerPort.ShortcutNode.Rect.Y + OwnerPort.ShortcutNode.Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                            ));
                        state.DrawingContext.DrawRectangle(state.ShortcutWirePen.Brush, null, new Rect(
                            state.WorldMatrix.Transform(new Point(targetX - 5, Rect.Y + Self.Rect.Y + 1)), new Size(10 * state.Scale, 10 * state.Scale)
                            ));
                    }
                }
            }
        }
    }
}