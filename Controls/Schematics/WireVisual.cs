using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Library.Drawing;

namespace LevelEditorPlugin.Controls
{
    public class WireVisual
    {
        public GeometryGroup WireGeometry => m_geometry;
        public BaseNodeVisual.Port HoveredPort => m_hoveredPort;
        
        public object Data;
        public BaseNodeVisual Source;
        public BaseNodeVisual.Port SourcePort;
        public BaseNodeVisual Target;
        public BaseNodeVisual.Port TargetPort;
        public int WireType;
        public int ConnectOrder;
        public List<WirePointVisual> WirePoints = new List<WirePointVisual>();
        public bool IsMarkedForDeletion = false;
        
        private GeometryGroup m_geometry;
        private PathGeometry m_hitTestGeometry;
        private Stopwatch m_glowTimer;

        private Point m_mousePosition;

        private BaseNodeVisual m_hoveredNode;
        private BaseNodeVisual.Port m_hoveredPort;
        
        private static Pen m_hitTestPen;

        public WireVisual(BaseNodeVisual source, string sourceName, int sourceHash, BaseNodeVisual target, string targetName, int targetHash, int portType)
        {
            Source = source;
            Target = target;
            WireType = portType;

            if (Source != null)
            {
                SourcePort = Source.Connect(this, sourceName, sourceHash, portType, 1);
            }
            if (Target != null)
            {
                TargetPort = Target.Connect(this, targetName, targetHash, portType, 0);
            }

            if (Source != null && Target != null)
            {
                ConnectOrder = (WireType == 0) ? TargetPort.ConnectionCount - 1 : SourcePort.ConnectionCount - 1;

                m_geometry = new GeometryGroup();
                m_geometry.Children.Add(new LineGeometry(Source.Rect.Location, Target.Rect.Location));

                if (m_hitTestPen == null)
                {
                    m_hitTestPen = new Pen(Brushes.Transparent, 4.0);
                    m_hitTestPen.Freeze();
                }

                m_glowTimer = new Stopwatch();
            }
        }

        public SchematicsLayout.Wire GenerateLayout()
        {
            SchematicsLayout.Wire wireLayout = new SchematicsLayout.Wire();

            SchematicsLayout.Node sourceNode = Source.GenerateLayout();
            SchematicsLayout.Node targetNode = Target.GenerateLayout();

            wireLayout.WirePoints = new List<SchematicsLayout.WirePoint>();

            foreach (WirePointVisual wirePoint in WirePoints)
            {
                SchematicsLayout.WirePoint wirePointLayout = new SchematicsLayout.WirePoint();
                wirePointLayout.UniqueId = wirePoint.UniqueId;
                wirePointLayout.Position = wirePoint.Rect.Location;
                wireLayout.WirePoints.Add(wirePointLayout);
            }

            return wireLayout;
        }

        public void ApplyLayout(SchematicsLayout.Wire wireLayout, List<BaseVisual> visuals)
        {
            foreach (SchematicsLayout.WirePoint wirePointLayout in wireLayout.WirePoints)
            {
                WirePointVisual wp = new WirePointVisual(this, wirePointLayout.Position.X + 5, wirePointLayout.Position.Y + 5) { UniqueId = (wirePointLayout.UniqueId != Guid.Empty) ? wirePointLayout.UniqueId : Guid.NewGuid() };
                visuals.Add(wp);

                AddWirePoint(wp, shouldForceAdd: true);
            }
        }

        public bool OnMouseMove(Point mousePos)
        {
            m_mousePosition = mousePos;
            return false;
        }

        public bool OnMouseDown(Point mousePos, MouseButton mousebutton)
        {
            return false;
        }

        public bool OnMouseUp(Point mousePos, MouseButton mouseButton)
        {
            if (m_hitTestGeometry != null)
            {
                if (m_hitTestGeometry.FillContains(mousePos) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    return true;
                }
            }
            if (Source == null || Target == null)
            {
                if (m_hoveredNode != null)
                {
                    if (m_hoveredNode is NodeVisual)
                    {
                        foreach (BaseNodeVisual.Port port in (m_hoveredNode as NodeVisual).AllPorts)
                        {
                            port.ShowWhileCollapsed = false;
                        }
                        m_hoveredNode.Update();
                    }
                    m_hoveredNode = null;
                }
            }
            return false;
        }

        public void UpdateEditing(Point mousePos, IEnumerable<BaseVisual> visibleNodes)
        {
            m_mousePosition = mousePos;

            BaseNodeVisual.Port wirePort = (Source != null) ? SourcePort : TargetPort;
            BaseNodeVisual oldHoveredNode = m_hoveredNode;

            m_hoveredNode = null;
            m_hoveredPort = null;

            foreach (BaseVisual visual in visibleNodes)
            {
                if (visual is NodeVisual)
                {
                    NodeVisual node = visual as NodeVisual;
                    if (node.Rect.Contains(mousePos))
                    {
                        m_hoveredNode = node;
                        foreach (BaseNodeVisual.Port port in node.AllPorts.Where(p => p.PortType == WireType && p.PortDirection != wirePort.PortDirection))
                        {
                            if (port.DataType == wirePort.DataType || wirePort.DataType == typeof(Any) || port.DataType == typeof(Any))
                            {
                                port.IsHighlighted = true;
                            }

                            Rect portRect = new Rect(node.Rect.X + port.Rect.X, node.Rect.Y + port.Rect.Y, port.Rect.Width + port.Name.Length * node.GlyphWidth, port.Rect.Height);
                            if (port.PortDirection == 0) portRect.X -= port.Name.Length * node.GlyphWidth;

                            if (portRect.Contains(mousePos))
                            {
                                m_hoveredPort = port;
                                m_mousePosition = node.Rect.Location;
                                m_mousePosition.X += port.Rect.Location.X + ((port.PortDirection == 0) ? 12 : 0);
                                m_mousePosition.Y += port.Rect.Location.Y + 6;
                            }
                            port.ShowWhileCollapsed = true;
                        }
                        m_hoveredNode.Update();
                        break;
                    }
                }
                else if (visual is InterfaceNodeVisual)
                {
                    InterfaceNodeVisual node = visual as InterfaceNodeVisual;
                    if (node.Rect.Contains(mousePos))
                    {
                        if (node.Self.PortType == WireType && node.Direction != wirePort.PortDirection)
                        {
                            m_hoveredNode = node;
                            Rect portRect = new Rect(node.Rect.X + node.Self.Rect.X, node.Rect.Y + node.Self.Rect.Y, node.Self.Rect.Width + node.Self.Name.Length * node.GlyphWidth, node.Self.Rect.Height);
                            if (node.Self.PortDirection == 0) portRect.X -= node.Self.Name.Length * node.GlyphWidth;

                            if (portRect.Contains(mousePos))
                            {
                                m_hoveredPort = node.Self;
                                m_mousePosition = node.Rect.Location;
                                m_mousePosition.X += node.Self.Rect.Location.X + ((node.Self.PortDirection == 0) ? 12 : 0);
                                m_mousePosition.Y += node.Self.Rect.Location.Y + 6;
                            }

                            m_hoveredNode.Update();
                            break;
                        }
                    }
                }
            }

            if (oldHoveredNode != m_hoveredNode && oldHoveredNode != null)
            {
                if (oldHoveredNode is NodeVisual)
                {
                    foreach (BaseNodeVisual.Port port in (oldHoveredNode as NodeVisual).AllPorts)
                    {
                        port.IsHighlighted = false;
                        port.ShowWhileCollapsed = false;
                    }
                    oldHoveredNode.Update();
                }
            }
        }

        public void Disconnect()
        {
            if (Source != null)
            {
                SourcePort.ConnectionCount--;
                SourcePort.Connections.Remove(this);
                if (SourcePort.ConnectionCount == 0)
                {
                    SourcePort.IsConnected = false;
                }
            }
            if (Target != null)
            {
                TargetPort.ConnectionCount--;
                TargetPort.Connections.Remove(this);
                if (TargetPort.ConnectionCount == 0)
                {
                    TargetPort.IsConnected = false;
                }
            }
        }

        public void AddWirePoint(WirePointVisual wirePoint, bool shouldForceAdd = false)
        {
            if (!shouldForceAdd)
            {
                AddWirePointInternal(wirePoint);
                return;
            }

            wirePoint.IsSelected = false;
            WirePoints.Add(wirePoint);
            m_geometry.Children.Add(new LineGeometry(Source.Rect.Location, Target.Rect.Location));
        }

        private void AddWirePointInternal(WirePointVisual wirePoint)
        {
            int index = 0;
            foreach (Geometry geom in m_geometry.Children)
            {
                if (geom.GetWidenedPathGeometry(m_hitTestPen).FillContains(new Point(wirePoint.Rect.X + 5, wirePoint.Rect.Y + 5)))
                {
                    WirePoints.Insert(index, wirePoint);
                    m_geometry.Children.Add(new LineGeometry(Source.Rect.Location, Target.Rect.Location));

                    break;
                }
                index++;
            }

            index++;
        }

        public void RemoveWirePoint(WirePointVisual wirePoint)
        {
            int index = WirePoints.IndexOf(wirePoint);

            WirePoints.RemoveAt(index);
            m_geometry.Children.RemoveAt(index + 1);
        }

        public void Render(SchematicsCanvas.DrawingContextState state)
        {
            BaseNodeVisual.Port portToCheck = (WireType == 0) ? TargetPort : SourcePort;
            bool shouldDrawConnectOrder = (state.ConnectorOrdersVisible && portToCheck.ConnectionCount > 1 && state.InvScale < 2.5);

            Point a = (Source != null) ? Source.Rect.Location : m_mousePosition;
            Point b = (Target != null) ? Target.Rect.Location : m_mousePosition;

            Pen wirePen = new Pen()
            {
                Brush = Brushes.Black, // default, so you know its wrong
                DashStyle = new DashStyle(null, 0.0d),
                DashCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Miter,
                MiterLimit = 1.0d,
                StartLineCap = PenLineCap.Round,
                Thickness = 2d * state.Scale // scales
            };

            if ((Source != null && Source.IsSelected) || (Target != null && Target.IsSelected))
            {
                wirePen.Brush = state.WireSelectedPen.Brush;
            }
            else
            {
                switch (WireType)
                {
                    case 0: wirePen.Brush = state.WireLinkPen.Brush; break;
                    case 1: wirePen.Brush = state.WireEventPen.Brush; break;
                    case 2: wirePen.Brush = state.WirePropertyPen.Brush; break;
                }
            }

            // highlight if overlapped by the cutting tool
            if (IsMarkedForDeletion)
            {
                wirePen.Brush = state.WireDeletingPen.Brush;
            }
            wirePen.Freeze();

            if (Source != null)
            {
                a.X += SourcePort.Rect.Location.X + 12;
                a.Y += SourcePort.Rect.Location.Y + 6;
            }
            if (Target != null)
            {
                b.X += TargetPort.Rect.Location.X;
                b.Y += TargetPort.Rect.Location.Y + 6;
            }

            Point ab = new Point(state.UseCurvedLines ? a.X + 4.5 : a.X + 9, a.Y);
            Point ba = new Point(state.UseCurvedLines ? b.X - 4.5 : b.X - 9, b.Y);
            state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(a), state.WorldMatrix.Transform(ab));

            if (WirePoints.Count > 0)
            {
                Point startPoint = ab;
                for (int i = 0; i < WirePoints.Count; i++)
                {
                    Point endPoint = new Point(WirePoints[i].Rect.Location.X + 5, WirePoints[i].Rect.Location.Y + 5);

                    (m_geometry.Children[i] as LineGeometry).StartPoint = startPoint;
                    (m_geometry.Children[i] as LineGeometry).EndPoint = endPoint;

                    if (state.UseCurvedLines)
                    {
                        state.DrawCurvedLine(wirePen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(endPoint));
                    }
                    else
                    {
                        state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(endPoint));
                    }

                    startPoint = endPoint;
                }

                (m_geometry.Children[WirePoints.Count] as LineGeometry).StartPoint = startPoint;
                (m_geometry.Children[WirePoints.Count] as LineGeometry).EndPoint = ba;

                if (state.UseCurvedLines)
                {
                    state.DrawCurvedLine(wirePen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(ba));
                }
                else
                {
                    state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(ba));
                }
            }
            else
            {
                if (Source != null && Target != null)
                {
                    (m_geometry.Children[0] as LineGeometry).StartPoint = ab;
                    (m_geometry.Children[0] as LineGeometry).EndPoint = ba;
                }

                if (state.UseCurvedLines)
                {
                    state.DrawCurvedLine(wirePen, state.WorldMatrix.Transform(ab), state.WorldMatrix.Transform(ba));
                }
                else
                {
                    state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(ab), state.WorldMatrix.Transform(ba));
                }
            }

            if (shouldDrawConnectOrder)
            {
                int geomIndex = (m_geometry.Children.Count / 2);
                Geometry geomToUse = m_geometry.Children[geomIndex];
                string connectorString; Point textCenter;
                if (state.UseAlternateConnectorOrders)
                {
                    Rect box = new Rect(state.WorldMatrix.Transform(new Point(geomToUse.Bounds.X + (geomToUse.Bounds.Width / 2) - 8, geomToUse.Bounds.Y + (geomToUse.Bounds.Height / 2) - 5)), new Size(16 * state.Scale, 10 * state.Scale));
                    state.DrawingContext.DrawRoundedRectangle(state.BlackPen.Brush, wirePen, box, 3, 3);

                    connectorString = (ConnectOrder + 1).ToString() + "/" + portToCheck.ConnectionCount.ToString();
                    textCenter = state.WorldMatrix.Transform(new Point(geomToUse.Bounds.X + (geomToUse.Bounds.Width / 2) - (state.SmallFont.OriginalAdvanceWidth * connectorString.Length) * 0.5, geomToUse.Bounds.Y + (geomToUse.Bounds.Height / 2) - state.SmallFont.OriginalAdvanceHeight * 0.5));
                    GlyphRun text = state.ConvertTextLinesToGlyphRun(textCenter, false, connectorString);
                    state.DrawingContext.DrawGlyphRun(Brushes.White, text);
                }
                else
                {
                    Rect box = new Rect(state.WorldMatrix.Transform(new Point(geomToUse.Bounds.X + (geomToUse.Bounds.Width / 2) - 6, geomToUse.Bounds.Y + (geomToUse.Bounds.Height / 2) - 5)), new Size(12 * state.Scale, 10 * state.Scale));
                    state.DrawingContext.DrawRectangle(wirePen.Brush, state.BlackPen, box);

                    connectorString = (ConnectOrder + 1).ToString();
                    textCenter = state.WorldMatrix.Transform(new Point(geomToUse.Bounds.X + (geomToUse.Bounds.Width / 2) - (state.SmallFont.OriginalAdvanceWidth * connectorString.Length) * 0.5, geomToUse.Bounds.Y + (geomToUse.Bounds.Height / 2) - state.SmallFont.OriginalAdvanceHeight * 0.5));
                    GlyphRun text = state.ConvertTextLinesToGlyphRun(textCenter, false, connectorString);
                    state.DrawingContext.DrawGlyphRun(Brushes.Black, text);
                }
            }

            state.DrawingContext.DrawLine(wirePen, state.WorldMatrix.Transform(ba), state.WorldMatrix.Transform(b));

            if (Source != null && Target != null)
            {
                m_hitTestGeometry = m_geometry.GetWidenedPathGeometry(state.WireHitTestPen);
            }
        }

        public void RenderDebug(SchematicsCanvas.DrawingContextState state)
        {
            if (WireType == 2)
            {
                IProperty propA = (Source is NodeVisual)
                    ? (Source as NodeVisual).Entity.GetProperty(SourcePort.NameHash)
                    : (Source as InterfaceNodeVisual).InterfaceDescriptor.GetProperty(SourcePort.NameHash);

                if (propA != null)
                {
                    if (propA.SetOnFrame >= state.LastFrameCount)
                    {
                        m_glowTimer.Restart();
                    }
                }
            }
            else if (WireType == 1)
            {
                IEvent eventA = (Source is NodeVisual)
                    ? (Source as NodeVisual).Entity.GetEvent(SourcePort.NameHash)
                    : (Source as InterfaceNodeVisual).InterfaceDescriptor.GetEvent(SourcePort.NameHash);

                if (eventA != null)
                {
                    if (eventA.SetOnFrame >= state.LastFrameCount)
                    {
                        m_glowTimer.Restart();
                    }
                }
            }

            if (m_glowTimer.IsRunning)
            {
                if (m_glowTimer.Elapsed.TotalSeconds > 1)
                {
                    m_glowTimer.Stop();
                    return;
                }

                Point a = Source.Rect.Location;
                Point b = Target.Rect.Location;

                a.X += SourcePort.Rect.Location.X + 12;
                a.Y += SourcePort.Rect.Location.Y + 6;

                b.X += TargetPort.Rect.Location.X;
                b.Y += TargetPort.Rect.Location.Y + 6;

                Point ab = new Point(state.UseCurvedLines ? a.X + 4.5 : a.X + 9, a.Y);
                Point ba = new Point(state.UseCurvedLines ? b.X - 4.5 : b.X - 9, b.Y);

                state.DrawingContext.PushOpacity((1 - m_glowTimer.Elapsed.TotalSeconds) * 0.5);
                state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(a), state.WorldMatrix.Transform(ab));

                if (WirePoints.Count > 0)
                {
                    Point startPoint = ab;
                    for (int i = 0; i < WirePoints.Count; i++)
                    {
                        Point endPoint = new Point(WirePoints[i].Rect.Location.X + 5, WirePoints[i].Rect.Location.Y + 5);

                        (m_geometry.Children[i] as LineGeometry).StartPoint = startPoint;
                        (m_geometry.Children[i] as LineGeometry).EndPoint = endPoint;

                        if (state.UseCurvedLines)
                        {
                            state.DrawCurvedLine(state.GlowPen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(endPoint));
                        }
                        else
                        {
                            state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(endPoint));
                        }

                        startPoint = endPoint;
                    }

                    (m_geometry.Children[WirePoints.Count] as LineGeometry).StartPoint = startPoint;
                    (m_geometry.Children[WirePoints.Count] as LineGeometry).EndPoint = ba;

                    if (state.UseCurvedLines)
                    {
                        state.DrawCurvedLine(state.GlowPen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(ba));
                    }
                    else
                    {
                        state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(startPoint), state.WorldMatrix.Transform(ba));
                    }
                }
                else
                {
                    (m_geometry.Children[0] as LineGeometry).StartPoint = ab;
                    (m_geometry.Children[0] as LineGeometry).EndPoint = ba;

                    if (state.UseCurvedLines)
                    {
                        state.DrawCurvedLine(state.GlowPen, state.WorldMatrix.Transform(ab), state.WorldMatrix.Transform(ba));
                    }
                    else
                    {
                        state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(ab), state.WorldMatrix.Transform(ba));
                    }
                }

                state.DrawingContext.DrawLine(state.GlowPen, state.WorldMatrix.Transform(ba), state.WorldMatrix.Transform(b));
                state.DrawingContext.Pop();
            }
        }
    }
}