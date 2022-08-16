using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Library.Drawing;
using LevelEditorPlugin.Library.Reflection;
using LevelEditorPlugin.Library.Schematics;
using LevelEditorPlugin.Managers;

namespace LevelEditorPlugin.Controls
{
    public class NodeVisual : BaseNodeVisual
    {
        public override object Data => Entity;

        public bool IsCollapsed;
        public bool IsCollapseButtonHovered;
        public int Realm;

        public List<Port> InputLinks = new List<Port>();
        public List<Port> OutputLinks = new List<Port>();
        public List<Port> InputEvents = new List<Port>();
        public List<Port> OutputEvents = new List<Port>();
        public List<Port> InputProperties = new List<Port>();
        public List<Port> OutputProperties = new List<Port>();
        public Port Self;

        public IEnumerable<Port> AllPorts => InputLinks.Concat(OutputLinks.Concat(InputEvents.Concat(OutputEvents.Concat(InputProperties.Concat(OutputProperties.Concat(new[] { Self }))))));

        public ILogicEntity Entity;

        public ContextMenu NodeContextMenu;
        public ContextMenu PortContextMenu;

        public NodeVisual(ILogicEntity entity, double x, double y)
            : base(x, y)
        {
            Entity = entity;
            Title = Entity.DisplayName;
            IsCollapsed = true;
            IsSelected = false;
            IsCollapseButtonHovered = false;

            // main connector
            Self = new Port(this) { Name = "", NameHash = 0, Rect = new Rect(-6, 4, 12, 12), PortType = 0, PortDirection = 1, DataType = Entity.GetType() };
        }

        public override bool OnMouseOver(Point mousePos)
        {
            Rect collapseButtonRect = new Rect(Rect.X + (Rect.Width - 16), Rect.Y + 4, 12, 12);
            bool oldCollapseHover = IsCollapseButtonHovered;

            IsCollapseButtonHovered = false;
            if (collapseButtonRect.Contains(mousePos))
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                IsCollapseButtonHovered = true;
            }

            bool changedHighlight = false;
            foreach (Port port in AllPorts)
            {
                Rect portRect = new Rect(Rect.X + port.Rect.X, Rect.Y + port.Rect.Y, port.Rect.Width, port.Rect.Height);
                if (portRect.Contains(mousePos))
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    HighlightedPort = port;
                    port.IsHighlighted = true;
                    changedHighlight = true;
                }
                else if (port.IsHighlighted)
                {
                    if (HighlightedPort == port)
                    {
                        HighlightedPort = null;
                    }
                    port.IsHighlighted = false;
                    changedHighlight = true;
                }
            }

            if (!IsCollapseButtonHovered && !changedHighlight)
            {
                base.OnMouseOver(mousePos);
            }

            return oldCollapseHover != IsCollapseButtonHovered || changedHighlight || !Rect.Contains(mousePos);
        }

        public override bool OnMouseDown(Point mousePos, MouseButton mouseButton)
        {
            // we don't want to pass mouse down input if we're hovering over collapse button
            if (mouseButton == MouseButton.Left)
            {
                if (IsCollapseButtonHovered)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool OnMouseUp(Point mousePos, MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                if (IsCollapseButtonHovered)
                {
                    IsCollapsed = !IsCollapsed;
                    Update();
                    return true;
                }
            }
            else if (mouseButton == MouseButton.Right)
            {
                foreach (Port port in AllPorts)
                {
                    if (!port.IsConnected || port.ShortcutNode != null)
                        continue;

                    Rect portRect = new Rect(Rect.X + port.Rect.X, Rect.Y + port.Rect.Y, 12, 12);
                    if (portRect.Contains(mousePos))
                    {
                        PortContextMenu.DataContext = port;
                        PortContextMenu.IsOpen = true;
                        return true;
                    }
                }

                if (NodeContextMenu != null && Rect.Contains(mousePos))
                {
                    NodeContextMenu.DataContext = this;
                    NodeContextMenu.IsOpen = true;
                    return true;
                }
            }

            return false;
        }

        public override bool OnMouseLeave()
        {
            bool invalidate = false;
            if (IsCollapseButtonHovered)
            {
                IsCollapseButtonHovered = false;
                invalidate = true;
            }
            if (HighlightedPort != null)
            {
                HighlightedPort.IsHighlighted = false;
                HighlightedPort = null;
                invalidate = true;
            }

            return invalidate;
        }

        public override void ApplyLayout(SchematicsLayout layout, SchematicsCanvas canvas)
        {
            SchematicsLayout.Node node = layout.Nodes.FirstOrDefault(n => n.FileGuid == Entity.FileGuid && n.InstanceGuid == Entity.InstanceGuid);
            if (node.FileGuid == Entity.FileGuid)
            {
                UniqueId = (node.UniqueId != Guid.Empty) ? node.UniqueId : UniqueId;
                IsCollapsed = node.IsCollapsed;
                Rect.X = node.Position.X;
                Rect.Y = node.Position.Y;

                if (node.ShortcutData.HasValue)
                {
                    foreach (SchematicsLayout.Shortcut shortcut in node.ShortcutData.Value.Shortcuts)
                    {
                        Port port = Self;
                        if (shortcut.ExtraData.HasValue)
                        {
                            port = AllPorts.FirstOrDefault(p => p.NameHash == shortcut.ExtraData.Value.NameHash && p.PortDirection == shortcut.ExtraData.Value.Direction && p.PortType == shortcut.ExtraData.Value.FieldType);
                        }
                        canvas.GenerateShortcuts(this, port, shortcut);
                    }
                }
            }
        }

        public override SchematicsLayout.Node GenerateLayout()
        {
            SchematicsLayout.Node node = new SchematicsLayout.Node();
            node.UniqueId = UniqueId;
            node.FileGuid = Entity.FileGuid;
            node.InstanceGuid = Entity.InstanceGuid;
            node.IsCollapsed = IsCollapsed;
            node.Position = Rect.Location;

            List<SchematicsLayout.Shortcut> shortcuts = new List<SchematicsLayout.Shortcut>();
            foreach (Port port in AllPorts)
            {
                if (port.ShortcutNode != null)
                {
                    SchematicsLayout.Shortcut shortcut = new SchematicsLayout.Shortcut();
                    shortcut.UniqueId = port.ShortcutNode.UniqueId;
                    shortcut.DisplayName = port.ShortcutNode.ShortcutData.DisplayName;
                    shortcut.Position = port.ShortcutNode.Rect.Location;
                    shortcut.ChildPositions = new List<Point>();
                    shortcut.ChildGuids = new List<Guid>();

                    if (port != Self)
                    {
                        SchematicsLayout.InterfaceData extraData = new SchematicsLayout.InterfaceData();
                        extraData.NameHash = port.NameHash;
                        extraData.Direction = port.PortDirection;
                        extraData.FieldType = port.PortType;
                        shortcut.ExtraData = extraData;
                    }

                    foreach (InterfaceShortcutNodeVisual child in port.ShortcutChildren)
                    {
                        shortcut.ChildPositions.Add(child.Rect.Location);
                        shortcut.ChildGuids.Add(child.UniqueId);
                    }

                    shortcuts.Add(shortcut);
                }
            }

            if (shortcuts.Count > 0)
            {
                SchematicsLayout.ShortcutData shortcutData = new SchematicsLayout.ShortcutData();
                shortcutData.Shortcuts = shortcuts;
                node.ShortcutData = shortcutData;
            }

            return node;
        }

        public override bool Matches(FrostySdk.Ebx.PointerRef pr, int nameHash, int direction)
        {
            Guid entityFileGuid = Entity.FileGuid;
            return pr.External.FileGuid == entityFileGuid && pr.External.ClassGuid == Entity.InstanceGuid;
        }

        public override bool IsValid()
        {
            if (ConnectionCount != 0)
                return true;

            if (Entity is ISpatialEntity && !(Entity is INotRealSpatialEntity))
                return false;

            if (Entity is IHideInSchematicsView)
                return false;

            return true;
        }

        public override Port Connect(WireVisual wire, string name, int nameHash, int portType, int direction)
        {
            List<Port> inputList = null;
            List<Port> outputList = null;

            if (nameHash == 0)
            {
                Self.IsConnected = true;
                Self.ConnectionCount++;
                Self.Connections.Add(wire);
                ConnectionCount++;

                return Self;
            }

            switch (portType)
            {
                case 0: inputList = InputLinks; outputList = OutputLinks; break;
                case 1: inputList = InputEvents; outputList = OutputEvents; break;
                case 2: inputList = InputProperties; outputList = OutputProperties; break;
            }

            Port port = null;
            switch (direction)
            {
                case 0: port = inputList.FirstOrDefault(p => p.NameHash == nameHash); break;
                case 1: port = outputList.FirstOrDefault(p => p.NameHash == nameHash); break;
            };

            // if port does not exist, create a dynamic one
            if (port == null)
            {
                // check to see if hash is found in strings.txt
                string foundName = FrostySdk.Utils.GetString(nameHash);
                if (foundName != name)
                {
                    name = foundName;
                    nameHash = SchematicsUtils.HashString(name);
                }

                port = new Port(this)
                {
                    Name = name,
                    NameHash = nameHash,
                    IsDynamicallyGenerated = true,
                    PortType = portType,
                    PortDirection = 1 - direction
                };

                if (direction == 0)
                {
                    inputList.Add(port);
                }
                else
                {
                    outputList.Add(port);
                }
            }

            port.IsConnected = true;
            port.ConnectionCount++;
            port.Connections.Add(wire);

            ConnectionCount++;
            return port;
        }

        public override void Update()
        {
            int connectorIdx = 0;
            int offset = 0;
            int linkCount = 0;
            int eventCount = 0;
            int propertyCount = 0;

            UpdatePorts();

            Realm = (int)Entity.Realm;

            Rect.Width = CalculateNodeWidth(out linkCount, out eventCount, out propertyCount);
            Rect.Height = 20 + 4;

            // header rows
            Rect.Height += Entity.HeaderRows.Count() * 10.0;
            double headerHeight = Rect.Height - 4;

            // input links
            foreach (Port port in InputLinks)
            {
                if (!IsCollapsed || port.IsConnected)
                {
                    port.Rect = new Rect(-6, ((headerHeight + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (linkCount != 0 && eventCount != 0) ? 5 : 0;
            connectorIdx = linkCount;

            // input events
            foreach (Port port in InputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(-6, ((headerHeight + offset + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (eventCount != 0 || linkCount != 0) ? 5 : 0;
            connectorIdx = eventCount + linkCount;

            // input properties
            foreach (Port port in InputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(-6, ((headerHeight + 4 + offset + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            connectorIdx = 0;
            offset = 0;

            // output links
            foreach (Port port in OutputLinks)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(Rect.Width + 6 - 12, ((headerHeight + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (linkCount != 0 && eventCount != 0) ? 5 : 0;
            connectorIdx = linkCount;

            // output events
            foreach (Port port in OutputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(Rect.Width + 6 - 12, ((headerHeight + offset + 4 + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            offset += (eventCount != 0 || linkCount != 0) ? 5 : 0;
            connectorIdx = eventCount + linkCount;

            // output properties
            foreach (Port port in OutputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    port.Rect = new Rect(Rect.Width + 6 - 12, ((headerHeight + 4 + offset + (15 * connectorIdx))), 12, 12);
                    connectorIdx++;
                }
            }

            Rect.Height += eventCount * 15 + propertyCount * 15 + linkCount * 15;

            if (linkCount > 0 && eventCount > 0)
                Rect.Height += 5;
            if ((eventCount > 0 || linkCount > 0) && propertyCount > 0)
                Rect.Height += 5;

            Rect.Height += 1;
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            DrawHeader(state);
            DrawBody(state);
        }

        public override void RenderDebug(SchematicsCanvas.DrawingContextState state)
        {
            IEnumerable<string> debugRows = Entity.DebugRows;
            if (debugRows.Count() == 0)
                return;

            double debugHeight = (debugRows.Count() * 10.0);

            Point nodePosition = state.WorldMatrix.Transform(new Point(Rect.Location.X, Rect.Location.Y - debugHeight - 9));
            Size nodeSize = new Size(Rect.Size.Width * state.Scale, (debugHeight + 5) * state.Scale);
            Point p = new Point(5, debugHeight + 5);

            if (state.InvScale < 1.5)
            {

                CombinedGeometry cg = new CombinedGeometry(
                    new RectangleGeometry(new Rect(nodePosition, nodeSize)),
                    new PathGeometry(new[]
                    {
                        new PathFigure(new Point(nodePosition.X + (p.X * state.Scale), nodePosition.Y + p.Y * state.Scale), new []
                        {
                            new PolyLineSegment(new []
                            {
                                new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + p.Y * state.Scale),
                                new Point(nodePosition.X + (p.X + 4) * state.Scale, nodePosition.Y + (p.Y + 4) * state.Scale)
                            }, true)

                        }, true)
                    }));

                state.DrawingContext.DrawGeometry(Brushes.White, state.BlackPen, cg);

                // header rows
                double offsetY = 5.0;
                foreach (string value in Entity.DebugRows)
                {
                    string rowValue = value;
                    if (rowValue.Length > 44)
                    {
                        rowValue = rowValue.Remove(44) + "...";
                    }

                    GlyphRun headerRowGlyphRun = state.ConvertTextLinesToGlyphRun(new Point(nodePosition.X + (5 * state.Scale), nodePosition.Y + ((offsetY - 0.5) * state.Scale)), false, rowValue);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, headerRowGlyphRun);
                    offsetY += 10.0;
                }
            }
        }

        private void UpdatePorts()
        {
            List<Port> oldInputLinks = new List<Port>();
            List<Port> oldOutputLinks = new List<Port>();
            List<Port> oldInputEvents = new List<Port>();
            List<Port> oldOutputEvents = new List<Port>();
            List<Port> oldInputProps = new List<Port>();
            List<Port> oldOutputProps = new List<Port>();

            oldInputLinks.AddRange(InputLinks);
            oldOutputLinks.AddRange(OutputLinks);
            oldInputEvents.AddRange(InputEvents);
            oldOutputEvents.AddRange(OutputEvents);
            oldInputProps.AddRange(InputProperties);
            oldOutputProps.AddRange(OutputProperties);

            // input links
            foreach (ConnectionDesc linkDesc in Entity.Links.Where(e => e.Direction == Direction.Out))
            {
                int hash = SchematicsUtils.HashString(linkDesc.Name);
                Port port = InputLinks.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    InputLinks.Add(new Port(this, linkDesc.DataType) { Name = ReflectionUtils.AddStringToList(linkDesc.Name), NameHash = hash, PortType = 0, PortDirection = 1 });
                }
                else
                {
                    port.DataType = linkDesc.DataType;
                    oldInputLinks.Remove(port);
                }
            }

            // input events
            foreach (ConnectionDesc eventDesc in Entity.Events.Where(e => e.Direction == Direction.In))
            {
                int hash = SchematicsUtils.HashString(eventDesc.Name);
                Port port = InputEvents.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    InputEvents.Add(new Port(this) { Name = ReflectionUtils.AddStringToList(eventDesc.Name), NameHash = hash, PortType = 1, PortDirection = 1 });
                }
                else
                {
                    oldInputEvents.Remove(port);
                }
            }

            // input properties
            foreach (ConnectionDesc propDesc in Entity.Properties.Where(p => p.Direction == Direction.In))
            {
                int hash = SchematicsUtils.HashString(propDesc.Name);
                Port port = InputProperties.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    InputProperties.Add(new Port(this, propDesc.DataType) { Name = ReflectionUtils.AddStringToList(propDesc.Name), NameHash = hash, PortType = 2, PortDirection = 1 });
                }
                else
                {
                    port.DataType = propDesc.DataType;
                    oldInputProps.Remove(port);
                }
            }

            // output links
            foreach (ConnectionDesc linkDesc in Entity.Links.Where(e => e.Direction == Direction.In))
            {
                int hash = SchematicsUtils.HashString(linkDesc.Name);
                Port port = OutputLinks.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    OutputLinks.Add(new Port(this, linkDesc.DataType) { Name = ReflectionUtils.AddStringToList(linkDesc.Name), NameHash = hash, PortType = 0, PortDirection = 0 });
                }
                else
                {
                    port.DataType = linkDesc.DataType;
                    oldOutputLinks.Remove(port);
                }
            }

            // output events
            foreach (ConnectionDesc eventDesc in Entity.Events.Where(e => e.Direction == Direction.Out))
            {
                int hash = SchematicsUtils.HashString(eventDesc.Name);
                Port port = OutputEvents.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    OutputEvents.Add(new Port(this) { Name = ReflectionUtils.AddStringToList(eventDesc.Name), NameHash = hash, PortType = 1, PortDirection = 0 });
                }
                else
                {
                    oldOutputEvents.Remove(port);
                }
            }

            // output properties
            foreach (ConnectionDesc propDesc in Entity.Properties.Where(p => p.Direction == Direction.Out))
            {
                int hash = SchematicsUtils.HashString(propDesc.Name);
                Port port = OutputProperties.Find(p => p.NameHash == hash);

                if (port == null)
                {
                    OutputProperties.Add(new Port(this, propDesc.DataType) { Name = ReflectionUtils.AddStringToList(propDesc.Name), NameHash = hash, PortType = 2, PortDirection = 0 });
                }
                else
                {
                    port.DataType = propDesc.DataType;
                    oldOutputProps.Remove(port);
                }
            }

            foreach (Port port in oldInputLinks) { if (!port.IsDynamicallyGenerated) { InputLinks.Remove(port); } }
            foreach (Port port in oldOutputLinks) { if (!port.IsDynamicallyGenerated) { OutputLinks.Remove(port); } }
            foreach (Port port in oldInputEvents) { if (!port.IsDynamicallyGenerated) { InputEvents.Remove(port); } }
            foreach (Port port in oldOutputEvents) { if (!port.IsDynamicallyGenerated) { OutputEvents.Remove(port); } }
            foreach (Port port in oldInputProps) { if (!port.IsDynamicallyGenerated) { InputProperties.Remove(port); } }
            foreach (Port port in oldOutputProps) { if (!port.IsDynamicallyGenerated) { OutputProperties.Remove(port); } }
        }
        
        private void DrawHeader(SchematicsCanvas.DrawingContextState state)
        {
            double headerHeight = (Entity.HeaderRows.Count() * 10.0) + 20;
            Point nodePosition = state.WorldMatrix.Transform(Rect.Location);
            Size nodeSize = Rect.Size;

            // title background
            state.DrawingContext.DrawRoundedRectangle(state.NodeTitleBackgroundBrush, null, new Rect(nodePosition.X, nodePosition.Y, Rect.Width * state.Scale, headerHeight * state.Scale), new CornerRadius(2 * state.Scale, 2 * state.Scale, 0, 0));

            if (state.InvScale >= 5)
            {
                return;
            }

            // self connector
            Brush brush = (Self.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicLinkBrush;
            Self.Draw(state, brush, state.NodeTitleBackgroundBrush, nodePosition);
            if (Self.IsHighlighted)
                DrawTooltip(state, Self);

            // @todo: cache the shapes

            // collapse button
            Brush collapseButtonBrush = IsCollapsed ? state.SchematicRealmDisabled : state.NodeCollapseButtonBackgroundBrush;

            Point p = new Point(nodeSize.Width - 16 + 3, 6);
            PathGeometry geometry;
            if (IsCollapsed)
            {
                geometry = new PathGeometry(new[]
                {
                    new PathFigure(new Point(nodePosition.X + (p.X * state.Scale), nodePosition.Y + (p.Y + 4) * state.Scale), new []
                    {
                        new PolyLineSegment(new []
                        {
                            // top right point
                            new Point(nodePosition.X + (p.X + 6) * state.Scale, nodePosition.Y + (p.Y - 1) * state.Scale),
                            // bottom right point
                            new Point(nodePosition.X + (p.X + 6) * state.Scale, nodePosition.Y + (p.Y + 9) * state.Scale)
                        }, true)

                    }, true)
                });
            }
            else
            {
                geometry = new PathGeometry(new[]
                {
                    new PathFigure(new Point(nodePosition.X + (p.X - 2) * state.Scale, nodePosition.Y + (p.Y + 1) * state.Scale), new []
                    {
                        new PolyLineSegment(new []
                        {
                            // right point
                            new Point(nodePosition.X + (p.X + 8) * state.Scale, nodePosition.Y + (p.Y + 1) * state.Scale),
                            // bottom middle point
                            new Point(nodePosition.X + (p.X + 3) * state.Scale, nodePosition.Y + (p.Y + 7) * state.Scale)
                        }, true)

                    }, true)
                });
            }
            state.DrawingContext.DrawGeometry(IsCollapseButtonHovered ? Brushes.White : state.SchematicRealmDisabled, null, geometry);

            // realm
            if (state.InvScale < 2)
            {
                Brush clientRealmBrush = Realm == 0 || Realm == 2 ? state.SchematicRealmEnabled : state.SchematicRealmDisabled;
                Brush serverRealmBrush = Realm == 1 || Realm == 2 ? state.SchematicRealmEnabled : state.SchematicRealmDisabled;

                // client
                Point clientPoint = new Point(nodeSize.Width - 30 + 1, 6);
                state.DrawingContext.DrawGeometry(clientRealmBrush, null, new PathGeometry(new[]
                {
                    new PathFigure(new Point(nodePosition.X + (clientPoint.X + 0) * state.Scale, nodePosition.Y + (clientPoint.Y + 0) * state.Scale), new []
                    {
                        new PolyLineSegment(new []
                        {
                            new Point(nodePosition.X + (clientPoint.X + 4) * state.Scale, nodePosition.Y + (clientPoint.Y + 0) * state.Scale),
                            new Point(nodePosition.X + (clientPoint.X + 3.75) * state.Scale, nodePosition.Y + (clientPoint.Y + 1) * state.Scale),
                            new Point(nodePosition.X + (clientPoint.X + 1) * state.Scale, nodePosition.Y + (clientPoint.Y + 1) * state.Scale),
                            new Point(nodePosition.X + (clientPoint.X + 1) * state.Scale, nodePosition.Y + (clientPoint.Y + 7) * state.Scale),
                            new Point(nodePosition.X + (clientPoint.X + 3.75) * state.Scale, nodePosition.Y + (clientPoint.Y + 7) * state.Scale),
                            new Point(nodePosition.X + (clientPoint.X + 4) * state.Scale, nodePosition.Y + (clientPoint.Y + 8) * state.Scale),
                            new Point(nodePosition.X + (clientPoint.X + 0) * state.Scale, nodePosition.Y + (clientPoint.Y + 8) * state.Scale)
                        }, true)

                    }, true)
                }));
                // server
                Point serverPoint = new Point(nodeSize.Width - 30 + 2, 6);
                state.DrawingContext.DrawGeometry(serverRealmBrush, null, new PathGeometry(new[]
                {
                    new PathFigure(new Point(nodePosition.X + (serverPoint.X + 4) * state.Scale, nodePosition.Y + (serverPoint.Y + 0) * state.Scale), new []
                    {
                        new PolyLineSegment(new []
                        {
                            new Point(nodePosition.X + (serverPoint.X + 8) * state.Scale, nodePosition.Y + (serverPoint.Y + 0) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 7.75) * state.Scale, nodePosition.Y + (serverPoint.Y + 1) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 5) * state.Scale, nodePosition.Y + (serverPoint.Y + 1) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 5) * state.Scale, nodePosition.Y + (serverPoint.Y + 3.5) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 8) * state.Scale, nodePosition.Y + (serverPoint.Y + 3.5) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 8) * state.Scale, nodePosition.Y + (serverPoint.Y + 8) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 4) * state.Scale, nodePosition.Y + (serverPoint.Y + 8) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 4.5) * state.Scale, nodePosition.Y + (serverPoint.Y + 7) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 7) * state.Scale, nodePosition.Y + (serverPoint.Y + 7) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 7) * state.Scale, nodePosition.Y + (serverPoint.Y + 4.5) * state.Scale),
                            new Point(nodePosition.X + (serverPoint.X + 4) * state.Scale, nodePosition.Y + (serverPoint.Y + 4.5) * state.Scale)
                        }, true)

                    }, true)
                }));
            }

            if (state.InvScale < 2.5)
            {
                // node title
                GlyphRun titleGlyphRun = state.ConvertTextLinesToGlyphRun(
                    new Point(
                        nodePosition.X + (8 * state.Scale),
                        nodePosition.Y + (3.5 * state.Scale)),
                    true,
                    (Entity as Entity).HasFlags(EntityFlags.HasLogic) ? Title : Title + "*");
                state.DrawingContext.DrawGlyphRun(Brushes.White, titleGlyphRun);

                if (state.InvScale < 1.5)
                {
                    // header rows
                    double offsetY = 20.0;
                    foreach (string value in Entity.HeaderRows)
                    {
                        string rowValue = value;
                        if (rowValue.Length > 44)
                        {
                            rowValue = rowValue.Remove(44) + "...";
                        }

                        if (value != "")
                        {
                            GlyphRun headerRowGlyphRun = state.ConvertTextLinesToGlyphRun(new Point(nodePosition.X + (5 * state.Scale), nodePosition.Y + ((offsetY - 0.5) * state.Scale)), false, rowValue);
                            state.DrawingContext.DrawGlyphRun(Brushes.White, headerRowGlyphRun);
                        }

                        offsetY += 10.0;
                    }
                }
            }
        }

        private void DrawBody(SchematicsCanvas.DrawingContextState state)
        {
            double headerHeight = (Entity.HeaderRows.Count() * 10.0) + 20;
            Point nodePosition = state.WorldMatrix.Transform(Rect.Location);
            Brush nodeBackgroundBrush = (IsSelected) ? state.NodeSelectedBrush : state.NodeBackgroundBrush;

            // node body background
            state.DrawingContext.DrawRoundedRectangle(nodeBackgroundBrush, null, new Rect(nodePosition.X, nodePosition.Y + headerHeight * state.Scale, Rect.Width * state.Scale, (Rect.Height - headerHeight) * state.Scale), new CornerRadius(0, 0, 2 * state.Scale, 2 * state.Scale));

            if (state.InvScale >= 5)
                return;

            // draw input links
            foreach (Port port in InputLinks)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicLinkBrush;
                port.Draw(state, brush, nodeBackgroundBrush, nodePosition);

                if (state.InvScale < 2.5)
                {
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + (port.Rect.X + 14) * state.Scale,
                            nodePosition.Y + port.Rect.Y * state.Scale),
                        true,
                        port.IsDynamicallyGenerated ? port.Name + "*" : port.Name);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }
            }
            // draw input events
            foreach (Port port in InputEvents)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicEventBrush;
                port.Draw(state, brush, nodeBackgroundBrush, nodePosition);

                if (state.InvScale < 2.5)
                {
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + (port.Rect.X + 14) * state.Scale,
                            nodePosition.Y + port.Rect.Y * state.Scale),
                        true,
                        port.IsDynamicallyGenerated ? port.Name + "*" : port.Name);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);
                }
            }
            // draw input properties
            foreach (Port port in InputProperties)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicPropertyBrush;
                port.Draw(state, brush, nodeBackgroundBrush, nodePosition);

                if (state.InvScale < 2.5)
                {
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + (port.Rect.X + 14) * state.Scale,
                            nodePosition.Y + port.Rect.Y * state.Scale),
                        true,
                        port.IsDynamicallyGenerated ? port.Name + "*" : port.Name);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }
            }

            // draw output links
            foreach (Port port in OutputLinks)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicLinkBrush;
                port.Draw(state, brush, nodeBackgroundBrush, nodePosition);

                if (state.InvScale < 2.5)
                {
                    string portName = port.IsDynamicallyGenerated ? port.Name + "*" : port.Name;
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + ((port.Rect.X - 4) * state.Scale) - portName.Length * state.LargeFont.AdvanceWidth,
                            nodePosition.Y + port.Rect.Y * state.Scale),
                        true,
                        portName);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }

            }
            // draw output events
            foreach (Port port in OutputEvents)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicEventBrush;
                port.Draw(state, brush, nodeBackgroundBrush, nodePosition);

                if (state.InvScale < 2.5)
                {
                    string portName = port.IsDynamicallyGenerated ? port.Name + "*" : port.Name;
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + ((port.Rect.X - 4) * state.Scale) - portName.Length * state.LargeFont.AdvanceWidth,
                            nodePosition.Y + port.Rect.Y * state.Scale),
                        true,
                        portName);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);
                }
            }
            // draw output properties
            foreach (Port port in OutputProperties)
            {
                if ((IsCollapsed && !port.IsConnected) && !port.ShowWhileCollapsed)
                    continue;

                Brush brush = (port.IsHighlighted) ? state.NodeSelectedBrush : state.SchematicPropertyBrush;
                port.Draw(state, brush, nodeBackgroundBrush, nodePosition);

                if (state.InvScale < 2.5)
                {
                    string portName = port.IsDynamicallyGenerated ? port.Name + "*" : port.Name;
                    GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(
                        new Point(
                            nodePosition.X + ((port.Rect.X - 4) * state.Scale) - portName.Length * state.LargeFont.AdvanceWidth,
                            nodePosition.Y + (port.Rect.Y) * state.Scale),
                        true,
                        portName);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, varGlypRun);

                    if (port.IsHighlighted)
                    {
                        DrawTooltip(state, port);
                    }
                }
            }
        }

        private void DrawTooltip(SchematicsCanvas.DrawingContextState state, Port port)
        {
            if (port.ShowWhileCollapsed)
                return;

            state.DrawingContext.PushOpacity(0.5);
            string dataTypeValue = (port.DataType != null) ? ConvertTypeName(port.DataType) : "Any";

            Rect rect = new Rect(
                port.Owner.Rect.X + port.Rect.X,
                port.Owner.Rect.Y + port.Rect.Y,
                5 + dataTypeValue.Length * GlyphWidth + 5,
                port.Rect.Height
                );

            if (port.PortDirection == 1) rect.X -= (5 + dataTypeValue.Length * GlyphWidth + 5) + 4;
            else rect.X += port.Rect.Width + 4;

            state.DrawingContext.DrawRectangle(state.NodeTitleBackgroundBrush, state.BlackPen, state.TransformRect(rect));

            rect.X += 5;
            rect.Y += 0;

            GlyphRun varGlypRun = state.ConvertTextLinesToGlyphRun(state.TransformPoint(rect.Location), true, dataTypeValue);
            state.DrawingContext.DrawGlyphRun(Brushes.White, varGlypRun);
            state.DrawingContext.Pop();
        }

        private string ConvertTypeName(Type inType)
        {
            switch (inType.Name)
            {
                case "Single": return "Float32";
                case "Int": return "Int32";
            }
            if (inType.Name.Equals("List`1"))
            {
                return $"{inType.GetGenericArguments()[0].Name}[]";
            }
            return inType.Name;
        }

        private double CalculateNodeWidth(out int linkCount, out int eventCount, out int propertyCount)
        {
            double nodeWidth = 8 + (Entity.DisplayName.Length * GlyphWidth) + 45;
            nodeWidth = nodeWidth - (nodeWidth % 10);

            double longestInput = 0.0;
            double longestOutput = 0.0;
            double longestHeaderRow = 0.0;

            int tmpEventCount = eventCount = 0;
            int tmpPropertyCount = propertyCount = 0;
            int tmpLinkCount = linkCount = 0;

            // links
            foreach (Port port in InputLinks)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestInput = (strLength > longestInput) ? strLength : longestInput; linkCount++;
                }
            }
            foreach (Port port in OutputLinks)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestOutput = (strLength > longestOutput) ? strLength : longestOutput; tmpLinkCount++;
                }
            }

            // events
            foreach (Port port in InputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestInput = (strLength > longestInput) ? strLength : longestInput; eventCount++;
                }
            }
            foreach (Port port in OutputEvents)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestOutput = (strLength > longestOutput) ? strLength : longestOutput; tmpEventCount++;
                }
            }

            // properties
            foreach (Port port in InputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestInput = (strLength > longestInput) ? strLength : longestInput; propertyCount++;
                }
            }
            foreach (Port port in OutputProperties)
            {
                if ((!IsCollapsed || port.IsConnected) || port.ShowWhileCollapsed)
                {
                    double strLength = port.Name.Length * GlyphWidth;
                    longestOutput = (strLength > longestOutput) ? strLength : longestOutput; tmpPropertyCount++;
                }
            }
            
            // header row
            foreach (string row in Entity.HeaderRows)
            {
                int strLen = (row.Length > 44) ? strLen = 47 : row.Length;
                double rowLength = strLen * (GlyphWidth * 0.6);
                longestHeaderRow = (rowLength > longestHeaderRow) ? rowLength : longestHeaderRow;
            }

            double textLength = longestInput + 20 + longestOutput + 20;
            textLength = (longestHeaderRow > textLength) ? longestHeaderRow : textLength;

            double newNodeWidth = 20 + textLength + 20;
            newNodeWidth = newNodeWidth - (newNodeWidth % 10);

            eventCount = (tmpEventCount > eventCount) ? tmpEventCount : eventCount;
            propertyCount = (tmpPropertyCount > propertyCount) ? tmpPropertyCount : propertyCount;
            linkCount = (tmpLinkCount > linkCount) ? tmpLinkCount : linkCount;

            return (nodeWidth > newNodeWidth) ? nodeWidth : newNodeWidth;
        }
    }
}