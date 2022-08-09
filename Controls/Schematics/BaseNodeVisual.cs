using System;
using System.Collections.Generic;
using System.Windows;
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
            public bool IsHighlighted;

            public int PortType;
            public int PortDirection;

            public InterfaceShortcutNodeVisual ShortcutNode;
            public List<InterfaceShortcutNodeVisual> ShortcutChildren = new List<InterfaceShortcutNodeVisual>();

            public Port(BaseNodeVisual owner)
            {
                Owner = owner;
            }

            public Port(BaseNodeVisual owner, Type dataType)
            {
                Owner = owner;
                DataType = dataType;
            }
        }

        public string Title;
        public double GlyphWidth;
        public int ConnectionCount;
        public Port HightlightedPort;

        public BaseNodeVisual(double x, double y)
            : base(x, y)
        {
        }

        public abstract void ApplyLayout(SchematicsLayout layout, SchematicsCanvas canvas);
        public abstract SchematicsLayout.Node GenerateLayout();
        public abstract bool Matches(FrostySdk.Ebx.PointerRef pr, int nameHash, int direction);
        public abstract bool IsValid();
        public abstract Port Connect(string name, int nameHash, int portType, int direction);

        protected int HashString(string value)
        {
            if (value.StartsWith("0x"))
                return int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
            return Frosty.Hash.Fnv1.HashString(value);
        }
    }
}