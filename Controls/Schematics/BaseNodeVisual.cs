using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LevelEditorPlugin.Managers;

namespace LevelEditorPlugin.Controls
{
    public abstract class BaseNodeVisual : BaseVisual
    {
        public class Port
        {
            public BaseNodeVisual Owner;
            public Rect Rect;
            public string Name;
            public int NameHash;
            public Type DataType;
            public bool IsConnected;
            public bool ShowWhileCollapsed;
            public bool IsDynamicallyGenerated;
            public int ConnectionCount;
            public List<WireVisual> Connections = new List<WireVisual>();
            public bool IsHighlighted;

            public int PortType;
            public int PortDirection;

            public InterfaceShortcutNodeVisual ShortcutNode;
            public List<InterfaceShortcutNodeVisual> ShortcutChildren = new List<InterfaceShortcutNodeVisual>();

            public Port(BaseNodeVisual inOwner)
            {
                Owner = inOwner;
            }

            public Port(BaseNodeVisual inOwner, Type inDataType)
            {
                Owner = inOwner;
                DataType = inDataType;
            }

            public void Draw(SchematicsCanvas.DrawingContextState state, Brush brush, Brush background, Point nodePosition)
            {
                if (!IsConnected)
                {
                    state.DrawingContext.DrawEllipse(background, null, new Point(nodePosition.X + (Rect.X + Rect.Width - 6) * state.Scale, nodePosition.Y + (Rect.Y + 6) * state.Scale), 6 * state.Scale, 6 * state.Scale);
                    state.DrawingContext.DrawEllipse(brush, state.BlackPen, new Point(nodePosition.X + (Rect.X + Rect.Width - 6) * state.Scale, nodePosition.Y + (Rect.Y + 6) * state.Scale), 3 * state.Scale, 3 * state.Scale);
                }
                else
                {
                    state.DrawingContext.DrawEllipse(brush, null, new Point(nodePosition.X + (Rect.X + Rect.Width - 6) * state.Scale, nodePosition.Y + (Rect.Y + 6) * state.Scale), 6 * state.Scale, 6 * state.Scale);
                }
            }
        }
        public string Title;
        public double GlyphWidth;
        public int ConnectionCount;
        public Port HighlightedPort;

        public BaseNodeVisual(double inX, double inY)
        : base(inX, inY)
        {
        }

        public abstract void ApplyLayout(SchematicsLayout layout, SchematicsCanvas canvas);
        public abstract SchematicsLayout.Node GenerateLayout();
        public abstract bool Matches(FrostySdk.Ebx.PointerRef pr, int nameHash, int direction);
        public abstract bool IsValid();
        public abstract Port Connect(WireVisual wire, string name, int nameHash, int portType, int direction);

        protected int HashString(string value)
        {
            if (value.StartsWith("0x"))
                return int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
            return Frosty.Hash.Fnv1.HashString(value);
        }
    }
}