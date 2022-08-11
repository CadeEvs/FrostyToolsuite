using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditorPlugin.Managers;
using FrostySdk.IO;
using LevelEditorPlugin.Data;
using LevelEditorPlugin.Library.Reflection;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using Frosty.Core.Managers;
using FrostySdk.Ebx;
using Entity = LevelEditorPlugin.Entities.Entity;

namespace LevelEditorPlugin.Controls
{
    public class NodeSorterHelper
    {
        private List<BaseVisual> nodes;
        private List<WireVisual> wires;

        public NodeSorterHelper(List<BaseVisual> inData, List<WireVisual> inWires)
        {
            nodes = inData;
            wires = inWires;
        }

        public void ApplyAutoLayout()
        {
            // Gather the nodes that feed into and out of each node
            Dictionary<BaseNodeVisual, List<BaseNodeVisual>> ancestors = new Dictionary<BaseNodeVisual, List<BaseNodeVisual>>();
            Dictionary<BaseNodeVisual, List<BaseNodeVisual>> children = new Dictionary<BaseNodeVisual, List<BaseNodeVisual>>();

            foreach (BaseVisual visual in nodes)
            {
                if (visual is BaseNodeVisual node)
                {
                    ancestors.Add(node, new List<BaseNodeVisual>());
                    children.Add(node, new List<BaseNodeVisual>());
                }
            }

            foreach (WireVisual wire in wires)
            {
                BaseNodeVisual source = wire.Source;
                BaseNodeVisual target = wire.Target;

                ancestors[target].Add(source);
                children[source].Add(target);
            }

            List<List<BaseNodeVisual>> columns = new List<List<BaseNodeVisual>>();
            List<BaseNodeVisual> alreadyProcessed = new List<BaseNodeVisual>();

            int columnIdx = 1;
            columns.Add(new List<BaseNodeVisual>());

            foreach (BaseNodeVisual node in nodes)
            {
                if (ancestors[node].Count == 0 && children[node].Count == 0)
                {
                    alreadyProcessed.Add(node);
                    columns[0].Add(node);
                    continue;
                }

                if (ancestors[node].Count == 0)
                {
                    LayoutNodes(node, children, columns, alreadyProcessed, columnIdx);
                }
            }

            columnIdx = 1;
            foreach (BaseNodeVisual node in nodes)
            {
                if (!alreadyProcessed.Contains(node))
                {
                    LayoutNodes(node, children, columns, alreadyProcessed, columnIdx);
                }
            }

            double x = 100.0;
            double width = 0.0;

            foreach (List<BaseNodeVisual> column in columns)
            {
                double y = 96.0;
                foreach (BaseNodeVisual node in column)
                {
                    node.Rect.X = x - (x % 8);
                    node.Rect.Y = y - (y % 8);
                    
                    node.Update();

                    double curWidth = Math.Floor((node.Rect.Width + 7.0) / 8.0) * 8.0;
                    double curHeight = Math.Floor((node.Rect.Height + 7.0) / 8.0) * 8.0;

                    y += curHeight + 56.0;
                    if (curWidth > width)
                    {
                        width = curWidth;
                    }
                }

                x += width + 60.0;
            }
        }

        private int LayoutNodes(BaseNodeVisual node, Dictionary<BaseNodeVisual, List<BaseNodeVisual>> children, List<List<BaseNodeVisual>> columns, List<BaseNodeVisual> alreadyProcessed, int column)
        {
            if (alreadyProcessed.Contains(node))
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i].Contains(node))
                        return i;
                }
            }

            alreadyProcessed.Add(node);
            if (columns.Count <= column)
                columns.Add(new List<BaseNodeVisual>());
            columns[column++].Add(node);

            int minimumColumn = 0;
            foreach (BaseNodeVisual child in children[node])
            {
                int tmp = LayoutNodes(child, children, columns, alreadyProcessed, column);
                if (tmp < minimumColumn || minimumColumn == 0)
                    minimumColumn = tmp;
            }

            if (minimumColumn > (column + 1))
            {
                columns[column - 1].Remove(node);
                columns[minimumColumn - 1].Add(node);
            }

            return column;
        }
    }
    
    public class SchematicsData
    {
        public ObservableCollection<Entity> Entities;
        public ObservableCollection<object> LinkConnections;
        public ObservableCollection<object> EventConnections;
        public ObservableCollection<object> PropertyConnections;
        public object InterfaceDescriptor;
        public Guid BlueprintGuid;
        public EntityWorld World;
    }

    public struct FontData
    {
        public GlyphTypeface GlyphTypeface;
        public double OriginalRenderingEmSize, RenderingEmSize, AdvanceWidth, AdvanceHeight;
        public double OriginalAdvanceWidth, OriginalAdvanceHeight;
        public Point BaselineOrigin;

        public static FontData MakeFont(Typeface typeface, double size)
        {
            FontData fontData = new FontData();
            typeface.TryGetGlyphTypeface(out fontData.GlyphTypeface);

            fontData.OriginalRenderingEmSize = size;
            fontData.RenderingEmSize = fontData.OriginalRenderingEmSize;
            fontData.AdvanceWidth = fontData.GlyphTypeface.AdvanceWidths[0] * fontData.RenderingEmSize;
            fontData.AdvanceHeight = fontData.GlyphTypeface.Height * fontData.RenderingEmSize;
            fontData.OriginalAdvanceHeight = fontData.AdvanceHeight;
            fontData.OriginalAdvanceWidth = fontData.AdvanceWidth;
            fontData.BaselineOrigin = new Point(0, fontData.GlyphTypeface.Baseline * fontData.RenderingEmSize);

            return fontData;
        }

        public void Update(double newScale)
        {
            RenderingEmSize = OriginalRenderingEmSize * newScale;
            AdvanceWidth = GlyphTypeface.AdvanceWidths[0] * RenderingEmSize;
            AdvanceHeight = GlyphTypeface.Height * RenderingEmSize;
            BaselineOrigin = new Point(0, GlyphTypeface.Baseline * RenderingEmSize);
        }
    }

    public class WireAddedEventArgs : EventArgs
    {
        public ILogicEntity SourceEntity { get; private set; }
        public ILogicEntity TargetEntity { get; private set; }
        public int SourceId { get; private set; }
        public int TargetId { get; private set; }
        public int ConnectionType { get; private set; }

        public WireAddedEventArgs(ILogicEntity inSource, ILogicEntity inTarget, int inSourceId, int inTargetId, int inConnectionType)
        {
            SourceEntity = inSource;
            TargetEntity = inTarget;
            SourceId = inSourceId;
            TargetId = inTargetId;
            ConnectionType = inConnectionType;
        }
    }

    public class NodeRemovedEventArgs : EventArgs
    {
        public ILogicEntity Entity { get; private set; }
        public UndoContainer Container { get; private set; }
        
        public NodeRemovedEventArgs(ILogicEntity inEntity, UndoContainer inContainer)
        {
            Entity = inEntity;
            Container = inContainer;
        }
    }
    
    public class NodeModifiedEventArgs : EventArgs
    {
    }

    public class SchematicsCanvas : GridCanvas
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(SchematicsData), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null, OnEntitiesChanged));
        public static readonly DependencyProperty ConnectorOrdersVisibleProperty = DependencyProperty.Register("ConnectorOrdersVisible", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty SuppressLayoutSaveProperty = DependencyProperty.Register("SuppressLayoutSave", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty SchematicLinkBrushProperty = DependencyProperty.Register("SchematicLinkBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SchematicEventBrushProperty = DependencyProperty.Register("SchematicEventBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SchematicPropertyBrushProperty = DependencyProperty.Register("SchematicPropertyBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SelectedNodeChangedCommandProperty = DependencyProperty.Register("SelectedNodeChangedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty WireAddedCommandProperty = DependencyProperty.Register("WireAddedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty NodeRemovedCommandProperty = DependencyProperty.Register("NodeRemovedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty NodeModifiedCommandProperty = DependencyProperty.Register("NodeModifiedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ModifiedDataProperty = DependencyProperty.Register("ModifiedData", typeof(object), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null, OnDataModified));
        public static readonly DependencyProperty UpdateDebugProperty = DependencyProperty.Register("UpdateDebug", typeof(object), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null, OnDebugUpdateModified));

        public object ModifiedData
        {
            get => GetValue(ModifiedDataProperty);
            set => SetValue(ModifiedDataProperty, value);
        }
        
        public SchematicsData ItemsSource
        {
            get => (SchematicsData)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public bool ConnectorOrdersVisible
        {
            get => (bool)GetValue(ConnectorOrdersVisibleProperty);
            set => SetValue(ConnectorOrdersVisibleProperty, value);
        }
        public bool SuppressLayoutSave
        {
            get => (bool)GetValue(SuppressLayoutSaveProperty);
            set => SetValue(SuppressLayoutSaveProperty, value);
        }
        public Brush SchematicLinkBrush
        {
            get => (Brush)GetValue(SchematicLinkBrushProperty);
            set => SetValue(SchematicLinkBrushProperty, value);
        }
        public Brush SchematicEventBrush
        {
            get => (Brush)GetValue(SchematicEventBrushProperty);
            set => SetValue(SchematicEventBrushProperty, value);
        }
        public Brush SchematicPropertyBrush
        {
            get => (Brush)GetValue(SchematicPropertyBrushProperty);
            set => SetValue(SchematicPropertyBrushProperty, value);
        }
        
        public ICommand SelectedNodeChangedCommand
        {
            get => (ICommand)GetValue(SelectedNodeChangedCommandProperty);
            set => SetValue(SelectedNodeChangedCommandProperty, value);
        }
        public ICommand WireAddedCommand
        {
            get => (ICommand)GetValue(WireAddedCommandProperty);
            set => SetValue(WireAddedCommandProperty, value);
        }
        public ICommand NodeRemovedCommand
        {
            get => (ICommand)GetValue(NodeRemovedCommandProperty);
            set => SetValue(NodeRemovedCommandProperty, value);
        }
        public ICommand NodeModifiedCommand
        {
            get => (ICommand)GetValue(NodeModifiedCommandProperty);
            set => SetValue(NodeModifiedCommandProperty, value);
        }
        
        public object UpdateDebug
        {
            get => GetValue(UpdateDebugProperty);
            set => SetValue(UpdateDebugProperty, value);
        }

        public double OriginalAdvanceWidth;

        private FontData largeFont;
        private FontData smallFont;

        private Pen wireLinkPen;
        private Pen wireEventPen;
        private Pen wirePropertyPen;
        private Pen wireHitTestPen;
        private Pen shortcutWirePen;
        private Pen wireSelectedPen;
        private Pen marqueePen;
        private Pen glowPen;

        private List<BaseVisual> nodeVisuals = new List<BaseVisual>();
        private List<BaseVisual> visibleNodeVisuals = new List<BaseVisual>();
        private List<WireVisual> wireVisuals = new List<WireVisual>();

        private List<BaseVisual> selectedNodes = new List<BaseVisual>();
        private List<Point> selectedOffsets = new List<Point>();

        private BaseVisual hoveredNode;

        private bool isMarqueeSelecting;
        private Point marqueeSelectionStart;
        private Point marqueeSelectionCurrent;

        private FormattedText simulationText;

        private DrawingVisual debugLayerVisual;
        private DrawingVisual debugWireVisual;

        private long lastFrameCount = -1;
        private WireVisual editingWire = null;

        public class DrawingContextState
        {
            public DrawingContext DrawingContext { get; private set; }

            // Options
            public bool ConnectorOrdersVisible { get; private set; }

            // Font
            public FontData LargeFont { get; private set; }
            public FontData SmallFont { get; private set; }

            // Matrices
            public MatrixTransform WorldMatrix { get; private set; }
            public MatrixTransform ViewMatrix { get; private set; }
            public double Scale { get; private set; }
            public double InvScale { get; private set; }

            // Various Pens cached
            public Pen BlackPen { get; private set; }
            public Pen GridMinorPen { get; private set; }
            public Pen GridMajorPen { get; private set; }
            public Pen WireLinkPen { get; private set; }
            public Pen WireEventPen { get; private set; }
            public Pen WirePropertyPen { get; private set; }
            public Pen WireHitTestPen { get; private set; }
            public Pen ShortcutWirePen { get; private set; }
            public Pen WireSelectedPen { get; private set; }
            public Pen GlowPen { get; private set; }

            // Various Brushes
            public Brush NodeTitleBackgroundBrush { get; private set; }
            public Brush NodeBackgroundBrush { get; private set; }
            public Brush NodeSelectedBrush { get; private set; }
            public Brush NodeCollapseButtonBackgroundBrush { get; private set; }
            public Brush SchematicLinkBrush { get; private set; }
            public Brush SchematicEventBrush { get; private set; }
            public Brush SchematicPropertyBrush { get; private set; }
            public Brush SchematicRealmEnabled { get; private set; }
            public Brush SchematicRealmDisabled { get; private set; }

            public long LastFrameCount { get; private set; }

            public static DrawingContextState CreateFromValues(DrawingContext inContext, MatrixTransform worldMatrix, double inScale)
            {
                DrawingContextState state = new DrawingContextState();
                state.DrawingContext = inContext;
                state.WorldMatrix = worldMatrix;
                state.ViewMatrix = new MatrixTransform();
                state.Scale = inScale;
                state.InvScale = 1.0 / inScale;

                state.BlackPen = new Pen(Brushes.Black, 1.0);
                
                state.NodeTitleBackgroundBrush = new SolidColorBrush(Color.FromRgb(63, 63, 63));
                state.NodeBackgroundBrush = new SolidColorBrush(Color.FromRgb(194, 194, 194));
                state.NodeCollapseButtonBackgroundBrush = new SolidColorBrush(Color.FromRgb(248, 248, 248));
                
                state.SchematicLinkBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x6f, 0xa9, 0xce));
                state.SchematicEventBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xf8, 0xf8, 0xf8));
                state.SchematicPropertyBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x5f, 0xd9, 0x5f));
                state.SchematicRealmEnabled = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                state.SchematicRealmDisabled = new SolidColorBrush(Color.FromRgb(155, 155, 155));

                state.LargeFont = FontData.MakeFont(new Typeface("Consolas"), 10);
                state.SmallFont = FontData.MakeFont(new Typeface("Consolas"), 7);

                return state;
            }

            private DrawingContextState()
            {
            }

            public DrawingContextState(SchematicsCanvas owner, DrawingContext currentContext)
            {
                DrawingContext = currentContext;

                WorldMatrix = owner.GetWorldMatrix();
                ViewMatrix = owner.GetViewMatrix();
                Scale = owner.scale;
                InvScale = 1.0 / owner.scale;

                ConnectorOrdersVisible = owner.ConnectorOrdersVisible;

                BlackPen = owner.blackPen;
                GridMinorPen = owner.gridMinorPen;
                GridMajorPen = owner.gridMajorPen;
                WireLinkPen = owner.wireLinkPen;
                WireEventPen = owner.wireEventPen;
                WirePropertyPen = owner.wirePropertyPen;
                WireHitTestPen = owner.wireHitTestPen;
                ShortcutWirePen = owner.shortcutWirePen;
                WireSelectedPen = owner.wireSelectedPen;
                GlowPen = owner.glowPen;

                NodeTitleBackgroundBrush = new SolidColorBrush(Color.FromRgb(63, 63, 63));
                NodeBackgroundBrush = new SolidColorBrush(Color.FromRgb(194, 194, 194));
                NodeSelectedBrush = Brushes.PaleGoldenrod;
                NodeCollapseButtonBackgroundBrush = new SolidColorBrush(Color.FromRgb(248, 248, 248));
                
                SchematicLinkBrush = owner.SchematicLinkBrush;
                SchematicEventBrush = owner.SchematicEventBrush;
                SchematicPropertyBrush = owner.SchematicPropertyBrush;
                SchematicRealmEnabled = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                SchematicRealmDisabled = new SolidColorBrush(Color.FromRgb(155, 155, 155));

                LastFrameCount = owner.lastFrameCount;

                LargeFont = owner.largeFont;
                SmallFont = owner.smallFont;
            }

            public Rect TransformRect(Rect rect)
            {
                return new Rect(WorldMatrix.Transform(rect.Location), new Size(rect.Width * Scale, rect.Height * Scale));
            }

            public Point TransformPoint(Point p)
            {
                return WorldMatrix.Transform(p);
            }

            public GlyphRun ConvertTextLinesToGlyphRun(Point position, bool large, string line)
            {
                FontData fontData = (large) ? LargeFont : SmallFont;

                List<ushort> glyphIndices = new List<ushort>();
                List<double> advanceWidths = new List<double>();
                List<Point> glyphOffsets = new List<Point>();

                double y = -position.Y;
                double x = position.X;

                for (int j = 0; j < line.Length; ++j)
                {
                    ushort glyphIndex = fontData.GlyphTypeface.CharacterToGlyphMap[line[j]];
                    glyphIndices.Add(glyphIndex);
                    advanceWidths.Add(0);
                    glyphOffsets.Add(new Point(x, y));

                    x += fontData.AdvanceWidth;
                }

                return new GlyphRun(
                    fontData.GlyphTypeface,
                    0,
                    false,
                    fontData.RenderingEmSize,
                    96.0f,
                    glyphIndices,
                    fontData.BaselineOrigin,
                    advanceWidths,
                    glyphOffsets,
                    null,
                    null,
                    null,
                    null,
                    null);
            }
        }

        public SchematicsCanvas()
        {
            largeFont = FontData.MakeFont(new Typeface("Consolas"), 10);
            smallFont = FontData.MakeFont(new Typeface("Consolas"), 7);
            OriginalAdvanceWidth = largeFont.AdvanceWidth;

            viewport = new TranslateTransform();

            Focusable = true;
            IsEnabled = true;

            simulationText = new FormattedText("SIMULATING", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 48.0, Brushes.White, 96.0);

            debugLayerVisual = new DrawingVisual();
            debugWireVisual = new DrawingVisual();
        }

        // @todo: organise this
        public void GenerateShortcuts(BaseNodeVisual node, BaseNodeVisual.Port port, SchematicsLayout.Shortcut? layoutData = null)
        {
            UndoContainer undoContainer = new UndoContainer("Make Shortcut");
            InterfaceShortcutNodeVisual shortcutNode = new InterfaceShortcutNodeVisual(node, port, new ShortcutParentNodeData() { DisplayName = port.Name }, 1 - port.PortDirection, 0, 0) { UniqueId = Guid.NewGuid() };

            double newX = node.Rect.X + ((port.PortDirection == 0) ? node.Rect.Width + 10 : -shortcutNode.Rect.Width - 10);
            double newY = node.Rect.Y + port.Rect.Y - 4;

            shortcutNode.Rect.Location = new Point(newX, newY);

            if (layoutData != null)
            {
                shortcutNode.UniqueId = (layoutData.Value.UniqueId != Guid.Empty) ? layoutData.Value.UniqueId : shortcutNode.UniqueId;
                shortcutNode.Rect.Location = layoutData.Value.Position;
                shortcutNode.ShortcutData.DisplayName = (layoutData.Value.DisplayName != null) ? layoutData.Value.DisplayName : shortcutNode.ShortcutData.DisplayName;
            }

            InterfaceShortcutNodeVisual parentShortcut = shortcutNode;
            undoContainer.Add(new GenericUndoUnit("",
                (o) => 
                { 
                    AddNode(parentShortcut); 
                    port.ShortcutNode = parentShortcut;
                    parentShortcut.Update();
                }, 
                (o) => 
                { 
                    nodeVisuals.Remove(parentShortcut); 
                    port.ShortcutNode = null; 
                }));
            
            int index = 0;
            int childIndex = 0;

            for (int i = wireVisuals.Count - 1; i >= 0; i--)
            {
                WireVisual wire = wireVisuals[i];
                if ((port.PortDirection == 0 && wire.SourcePort == port) || (port.PortDirection == 1 && wire.TargetPort == port))
                {
                    BaseNodeVisual targetNode = (port.PortDirection == 0) ? wire.Target : wire.Source;
                    BaseNodeVisual.Port targetPort = (port.PortDirection == 0) ? wire.TargetPort : wire.SourcePort;

                    shortcutNode = new InterfaceShortcutNodeVisual(node, port, new ShortcutChildNodeData() { DisplayName = port.Name }, port.PortDirection, 0, 0) { UniqueId = Guid.NewGuid() };

                    newY = targetNode.Rect.Y + targetPort.Rect.Y - 4;
                    newX = (targetNode.Rect.X + targetPort.Rect.X) + ((port.PortDirection == 0) ? -shortcutNode.Rect.Width - 10 : 30);

                    shortcutNode.Rect.Location = new Point(newX, newY);

                    if (layoutData != null)
                    {
                        if (index < layoutData.Value.ChildPositions.Count)
                        {
                            if (layoutData.Value.ChildGuids != null)
                            {
                                shortcutNode.UniqueId = (layoutData.Value.ChildGuids[index] != Guid.Empty) ? layoutData.Value.ChildGuids[index] : shortcutNode.UniqueId;
                            }
                            shortcutNode.Rect.Location = layoutData.Value.ChildPositions[index++];
                            shortcutNode.ShortcutData.DisplayName = (layoutData.Value.DisplayName != null) ? layoutData.Value.DisplayName : shortcutNode.ShortcutData.DisplayName;
                        }
                    }

                    InterfaceShortcutNodeVisual childToAdd = shortcutNode;
                    undoContainer.Add(new GenericUndoUnit("",
                        (o) => 
                        { 
                            AddNode(childToAdd);
                            port.ShortcutChildren.Add(childToAdd);
                            childToAdd.Update();
                        }, 
                        (o) => 
                        { 
                            nodeVisuals.Remove(childToAdd);
                            port.ShortcutChildren.Remove(childToAdd); 
                        }));

                    int indexToDelete = i;
                    WireVisual wireToDelete = wireVisuals[indexToDelete];

                    foreach (WirePointVisual wirePoint in wire.WirePoints)
                    {
                        undoContainer.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                nodeVisuals.Remove(wirePoint);
                            }, (o) =>
                            {
                                nodeVisuals.Add(wirePoint);
                                wireToDelete.WirePoints.Add(wirePoint);
                            }));
                    }

                    undoContainer.Add(new GenericUndoUnit("",
                        (o) => 
                        {
                            wireToDelete.WirePoints.Clear();
                            wireVisuals.Remove(wireToDelete);
                        }, 
                        (o) =>
                        {
                            wireVisuals.Insert(indexToDelete, wireToDelete);
                        }));

                    int childIndexToUse = childIndex;
                    WireVisual wireToModify = wire;

                    undoContainer.Add(new GenericUndoUnit("",
                        (o) =>
                        {
                            BaseNodeVisual sourceNode = (port.PortDirection == 0) ? port.ShortcutChildren[childIndexToUse] : wireToModify.Source;
                            targetNode = (port.PortDirection == 0) ? wireToModify.Target : port.ShortcutChildren[childIndexToUse];

                            WireVisual newWire = new WireVisual(sourceNode, wireToModify.SourcePort.Name, wireToModify.SourcePort.NameHash, targetNode, wireToModify.TargetPort.Name, wireToModify.TargetPort.NameHash, wireToModify.WireType);
                            wireVisuals.Add(newWire);
                        }, 
                        (o) =>
                        {
                            wireVisuals.RemoveAt(wireVisuals.Count - 1);
                        }));

                    childIndex++;
                }
            }

            {
                undoContainer.Add(new GenericUndoUnit("",
                    (o) =>
                    {
                        BaseNodeVisual sourceNode = (port.PortDirection == 0) ? node : port.ShortcutNode;
                        BaseNodeVisual.Port sourcePort = (port.PortDirection == 0) ? port : port.ShortcutNode.Self;

                        BaseNodeVisual targetNode = (port.PortDirection == 0) ? port.ShortcutNode : node;
                        BaseNodeVisual.Port targetPort = (port.PortDirection == 0) ? port.ShortcutNode.Self : port;

                        WireVisual newWire = new WireVisual(sourceNode, sourcePort.Name, sourcePort.NameHash, targetNode, targetPort.Name, targetPort.NameHash, port.PortType);
                        wireVisuals.Add(newWire);
                    }, 
                    (o) =>
                    {
                        wireVisuals.RemoveAt(wireVisuals.Count - 1);
                    }));
            }

            undoContainer.Add(new GenericUndoUnit("", o => InvalidateVisual(), o => ClearSelection()));

            UndoManager.Instance.CommitUndo(undoContainer);
            if (layoutData.HasValue)
            {
                UndoManager.Instance.Clear();
            }
        }

        public void UpdateDebugLayer()
        {
            lastFrameCount = ItemsSource.World.SimulationFrameCount;
            InvalidateVisual();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            wireLinkPen = new Pen(SchematicLinkBrush, 2.0); wireLinkPen.Freeze();
            wireEventPen = new Pen(SchematicEventBrush, 2.0); wireEventPen.Freeze();
            wirePropertyPen = new Pen(SchematicPropertyBrush, 2.0); wirePropertyPen.Freeze();
            wireHitTestPen = new Pen(Brushes.Transparent, 4.0); wireHitTestPen.Freeze();
            shortcutWirePen = new Pen(Brushes.WhiteSmoke, 2.0) { DashStyle = DashStyles.Dash }; shortcutWirePen.Freeze();
            wireSelectedPen = new Pen(Brushes.PaleGoldenrod, 2.0); wireSelectedPen.Freeze();
            marqueePen = new Pen(Brushes.SlateGray, 2.0) { DashStyle = DashStyles.Dash }; marqueePen.Freeze();
            glowPen = new Pen(Brushes.Yellow, 5.0); glowPen.Freeze();

            Unloaded += (o, ev) =>
            {
                if (SuppressLayoutSave)
                    return;

                SchematicsLayout layout = new SchematicsLayout();

                layout.FileGuid = ItemsSource.BlueprintGuid;
                layout.CanvasPosition = new Point(offset.X, offset.Y);
                layout.CanvasScale = scale;
                layout.Nodes = new List<SchematicsLayout.Node>();
                layout.Wires = new List<SchematicsLayout.Wire>();
                layout.Comments = new List<SchematicsLayout.Comment>();

                foreach (BaseVisual node in nodeVisuals)
                {
                    if (node is BaseNodeVisual && !(node is InterfaceShortcutNodeVisual))
                    {
                        layout.Nodes.Add((node as BaseNodeVisual).GenerateLayout());
                    }
                    else if (node is CommentNodeVisual)
                    {
                        layout.Comments.Add((node as CommentNodeVisual).GenerateLayout());
                    }
                }

                for (int i = 0; i < wireVisuals.Count; i++)
                {
                    if (wireVisuals[i].WirePoints.Count > 0)
                    {
                        SchematicsLayout.Wire wireLayout = wireVisuals[i].GenerateLayout();
                        wireLayout.Index = i;

                        layout.Wires.Add(wireLayout);
                    }
                }

                SchematicsLayoutManager.Instance.AddLayout(ItemsSource.BlueprintGuid, layout);
            };

            Focus();
        }

        protected void Update()
        {
            selectedNodes.Clear();
            selectedOffsets.Clear();

            hoveredNode = null;

            visibleNodeVisuals.Clear();
            nodeVisuals.Clear();
            wireVisuals.Clear();

            if (ItemsSource != null)
            {
                ItemsSource.Entities.CollectionChanged += Entities_CollectionChanged;
                ItemsSource.PropertyConnections.CollectionChanged += PropertyConnections_CollectionChanged;
                ItemsSource.EventConnections.CollectionChanged += EventConnections_CollectionChanged;
                ItemsSource.LinkConnections.CollectionChanged += LinkConnections_CollectionChanged;

                GenerateNodes(ItemsSource.Entities);
                GenerateInterface();
                GenerateWires(ItemsSource.LinkConnections, 0);
                GenerateWires(ItemsSource.EventConnections, 1);
                GenerateWires(ItemsSource.PropertyConnections, 2);

                // @temp: Remove any node spatial entity that doesnt have a recorded connection. Need to find a better
                // way to handle this for eliminating nodes that should not appear

                // apply custom layout if there is one, if not use the auto layout helper
                SchematicsLayout? layout = SchematicsLayoutManager.Instance.GetLayout(ItemsSource.BlueprintGuid);
                if (layout.HasValue)
                {
                    // canvas
                    scale = layout.Value.CanvasScale;
                    offset = new TranslateTransform(layout.Value.CanvasPosition.X, layout.Value.CanvasPosition.Y);
                    UpdateScaleParameters();
                    
                    // nodes
                    for (int i = nodeVisuals.Count - 1; i >= 0; i--)
                    {
                        if (nodeVisuals[i] is BaseNodeVisual nodeVisual)
                        {
                            nodeVisual.ApplyLayout(layout.Value, this);

                            // update the node to adjust for the collapsed state
                            nodeVisual.Update();
                        }
                    }
                    
                    // wires
                    if (layout.Value.Wires != null)
                    {
                        foreach (SchematicsLayout.Wire wireLayout in layout.Value.Wires)
                        {
                            wireVisuals[wireLayout.Index].ApplyLayout(wireLayout, nodeVisuals);
                        }
                    }
                    
                    // comments
                    if (layout.Value.Comments != null)
                    {
                        List<BaseVisual> comments = new List<BaseVisual>();
                        foreach (SchematicsLayout.Comment commentLayout in layout.Value.Comments)
                        {
                            List<BaseVisual> children = new List<BaseVisual>();
                            foreach (Guid guid in commentLayout.Children)
                            {
                                children.Add(nodeVisuals.Find(n => n.UniqueId == guid));
                            }

                            CommentNodeVisual commentVisual = new CommentNodeVisual(
                                new CommentNodeData()
                                {
                                    Color = new FrostySdk.Ebx.Vec4() { x = commentLayout.Color.R / 255.0f, y = commentLayout.Color.G / 255.0f, z = commentLayout.Color.B / 255.0f, w = 1.0f },
                                    CommentText = commentLayout.Text

                                }, children, commentLayout.Position.X, commentLayout.Position.Y);
                            commentVisual.UniqueId = commentLayout.UniqueId;
                            comments.Add(commentVisual);
                        }
                        nodeVisuals.AddRange(comments);
                    }
                }
                else
                {
                    NodeSorterHelper sorter = new NodeSorterHelper(nodeVisuals, wireVisuals);
                    sorter.ApplyAutoLayout();
                }
            }

            InvalidateVisual();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            // create shortcut
            if (selectedNodes.Count == 1 && selectedNodes[0] is InterfaceNodeVisual && !(selectedNodes[0] is InterfaceShortcutNodeVisual))
            {
                if (e.Key == Key.S)
                {
                    InterfaceNodeVisual node = selectedNodes[0] as InterfaceNodeVisual;
                    if (node.Self.ShortcutNode == null)
                    {
                        GenerateShortcuts(node, node.Self);
                    }

                    e.Handled = true;
                }
            }
            // delete node(s)
            else if (selectedNodes.Count > 0 && e.Key == Key.Delete)
            {
                UndoContainer container = new UndoContainer("Delete Node(s)");
                for (int i = selectedNodes.Count - 1; i >= 0; i--)
                {
                    BaseVisual node = selectedNodes[i];
                    
                    // wire point
                    if (node is WirePointVisual wirePoint)
                    {
                        container.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                wirePoint.Wire.RemoveWirePoint(wirePoint);
                                nodeVisuals.Remove(wirePoint);
                            }, 
                            (o) =>
                            {
                                wirePoint.Wire.AddWirePoint(wirePoint, force: true);
                                nodeVisuals.Add(wirePoint);
                            }));

                        selectedNodes.RemoveAt(i);
                        selectedOffsets.RemoveAt(i);
                    }
                    
                    // comment
                    if (node is CommentNodeVisual comment)
                    {
                        container.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                nodeVisuals.Remove(comment);
                            }, 
                            (o) =>
                            {
                                nodeVisuals.Add(comment);
                            }));

                        selectedNodes.RemoveAt(i);
                        selectedOffsets.RemoveAt(i);
                    }

                    // node
                    if (node is NodeVisual entity)
                    {
                        NodeRemovedCommand?.Execute(new NodeRemovedEventArgs(entity.Entity, container));
                    }
                }

                if (container.HasItems)
                {
                    container.Add(new GenericUndoUnit("", o => InvalidateVisual(), o => ClearSelection()));
                    UndoManager.Instance.CommitUndo(container);
                }

                e.Handled = true;
            }
            // create comment
            else if (e.Key == Key.G)
            {
                if (selectedNodes.Count > 0)
                {
                    CommentNodeVisual commentNode = new CommentNodeVisual(new CommentNodeData() { Color = new FrostySdk.Ebx.Vec4() { x = 0.25f, y = 0.0f, z = 0.0f, w = 1.0f }, CommentText = "@todo" }, selectedNodes, 0, 0) { UniqueId = Guid.NewGuid() };
                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Create Comment Group",
                        (o) =>
                        {
                            AddNode(commentNode);
                            InvalidateVisual();
                        },
                        (o) =>
                        {
                            nodeVisuals.Remove(commentNode);
                            ClearSelection();
                        }));
                }

                e.Handled = true;
            }
            // DEBUG: Invalidate Visual
            else if (e.Key == Key.R)
            {
                InvalidateVisual();
                Frosty.Core.App.NotificationManager.Show("Invalidated Visual");
            }

            base.OnPreviewKeyUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Focus();

                MatrixTransform m = GetWorldMatrix();
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                if (editingWire != null)
                {
                    editingWire.OnMouseMove(mousePos);
                    editingWire.UpdateEditing(mousePos, visibleNodeVisuals.Where(n => n is BaseNodeVisual));
                    InvalidateVisual();
                    return;
                }

                if (hoveredNode != null && hoveredNode is NodeVisual)
                {
                    if ((hoveredNode as NodeVisual).CollapseHover)
                        return;
                    if ((hoveredNode as NodeVisual).HightlightedPort != null)
                        return;
                }

                if (selectedNodes.Count > 0 && selectedNodes.Contains(hoveredNode))
                {
                    if (!UndoManager.Instance.IsUndoing && UndoManager.Instance.PendingUndoUnit == null)
                    {
                        List<BaseVisual> prevSelection = new List<BaseVisual>();
                        prevSelection.AddRange(selectedNodes);

                        List<Point> prevPositions = new List<Point>();
                        foreach (BaseVisual node in selectedNodes)
                            prevPositions.Add(node.Rect.Location);
                        
                        UndoManager.Instance.PendingUndoUnit = new GenericUndoUnit("Move Nodes",
                            (o) =>
                            {
                                // do nothing for now
                            },
                            (o) =>
                            {
                                for (int i = 0; i < prevSelection.Count; i++)
                                {
                                    prevSelection[i].Move(prevPositions[i]);
                                }
                                ClearSelection();
                            });
                    }

                    for (int i = 0; i < selectedNodes.Count; i++)
                    {
                        double newX = mousePos.X + selectedOffsets[i].X;
                        double newY = mousePos.Y + selectedOffsets[i].Y;

                        newX -= (newX % 5);
                        newY -= (newY % 5);

                        selectedNodes[i].Move(new Point(newX, newY));
                    }
                    InvalidateVisual();
                    return;
                }

                if (isMarqueeSelecting)
                {
                    marqueeSelectionCurrent = mousePos;

                    double width = marqueeSelectionCurrent.X - marqueeSelectionStart.X;
                    double height = marqueeSelectionCurrent.Y - marqueeSelectionStart.Y;
                    Point startPos = marqueeSelectionStart;

                    if (width <= 0)
                    {
                        width = marqueeSelectionStart.X - marqueeSelectionCurrent.X;
                        startPos.X = marqueeSelectionCurrent.X;
                    }
                    if (height <= 0)
                    {
                        height = marqueeSelectionStart.Y - marqueeSelectionCurrent.Y;
                        startPos.Y = marqueeSelectionCurrent.Y;
                    }

                    Rect selectionRect = new Rect(startPos, new Size(width, height));

                    for (int i = selectedNodes.Count - 1; i >= 0; i--)
                    {
                        if (!selectionRect.IntersectsWith(selectedNodes[i].Rect))
                        {
                            selectedNodes[i].IsSelected = false;
                            selectedNodes.RemoveAt(i);
                            selectedOffsets.RemoveAt(i);
                        }
                    }

                    foreach (BaseVisual node in nodeVisuals)
                    {
                        if (selectionRect.IntersectsWith(node.Rect))
                        {
                            if (!selectedNodes.Contains(node))
                            {
                                node.IsSelected = true;
                                selectedNodes.Add(node);
                                selectedOffsets.Add(new Point(node.Rect.Location.X - mousePos.X, node.Rect.Location.Y - mousePos.Y));
                            }
                        }
                    }

                    InvalidateVisual();
                }
            }
            else
            {
                MatrixTransform m = GetWorldMatrix();
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                foreach (BaseVisual visual in visibleNodeVisuals)
                {
                    if (visual.Rect.Contains(mousePos) && visual.HitTest(mousePos))
                    {
                        bool invalidate = false;
                        if (hoveredNode != null && hoveredNode != visual)
                        {
                            hoveredNode.OnMouseLeave();
                            invalidate = true;
                        }

                        hoveredNode = visual;
                        Mouse.OverrideCursor = Cursors.SizeAll;

                        if (visual.OnMouseOver(mousePos))
                        {
                            invalidate = true;
                        }

                        if (invalidate)
                        {
                            InvalidateVisual();
                        }

                        return;
                    }
                }

                if (hoveredNode != null)
                {
                    if (hoveredNode.OnMouseLeave())
                        InvalidateVisual();

                    hoveredNode = null;
                    Mouse.OverrideCursor = null;
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            MatrixTransform m = GetWorldMatrix();
            Point mousePos = m.Inverse.Transform(e.GetPosition(this));

            foreach (BaseVisual visual in visibleNodeVisuals)
            {
                if (visual.OnMouseDown(mousePos, e.ChangedButton))
                {
                    e.Handled = true;
                    InvalidateVisual();
                    return;
                }
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                prevMousePos = mousePos;

                if (hoveredNode != null)
                {
                    if (hoveredNode is BaseNodeVisual && (hoveredNode as BaseNodeVisual).HightlightedPort != null)
                    {
                        BaseNodeVisual node = hoveredNode as BaseNodeVisual;
                        BaseNodeVisual sourceNode = (node.HightlightedPort.PortDirection == 0) ? node : null;
                        BaseNodeVisual targetNode = (node.HightlightedPort.PortDirection == 1) ? node : null;

                        editingWire = new WireVisual(
                            sourceNode,
                            (sourceNode != null) ? sourceNode.HightlightedPort.Name : "",
                            (sourceNode != null) ? sourceNode.HightlightedPort.NameHash : 0,
                            targetNode,
                            (targetNode != null) ? targetNode.HightlightedPort.Name : "",
                            (targetNode != null) ? targetNode.HightlightedPort.NameHash : 0,
                            node.HightlightedPort.PortType
                            );
                        editingWire.OnMouseMove(mousePos);

                        InvalidateVisual();
                        return;
                    }

                    foreach (BaseVisual visual in visibleNodeVisuals)
                    {
                        if (visual.Rect.Contains(mousePos) && visual.HitTest(mousePos))
                        {
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                Mouse.Capture(this);

                                if (!selectedNodes.Contains(visual))
                                {
                                    selectedNodes.Add(visual);
                                    selectedOffsets.Add(new Point(visual.Rect.Location.X - mousePos.X, visual.Rect.Location.Y - mousePos.Y));

                                    visual.IsSelected = true;

                                    if (visual.Data != null)
                                    {
                                        SelectedNodeChangedCommand?.Execute(visual.Data);
                                    }

                                    InvalidateVisual();
                                }
                            }
                            else
                            {
                                if (!selectedNodes.Contains(visual))
                                {
                                    if (selectedNodes.Count > 0)
                                    {
                                        foreach (BaseVisual node in selectedNodes)
                                        {
                                            node.IsSelected = false;
                                        }

                                        selectedNodes.Clear();
                                        selectedOffsets.Clear();
                                    }

                                    selectedNodes.Add(visual);
                                    selectedOffsets.Add(new Point(visual.Rect.Location.X - mousePos.X, visual.Rect.Location.Y - mousePos.Y));

                                    visual.IsSelected = true;

                                    if (visual.Data != null)
                                    {
                                        SelectedNodeChangedCommand?.Execute(visual.Data);
                                    }
                                    InvalidateVisual();
                                }
                            }

                            for (int i = 0; i < selectedOffsets.Count; i++)
                            {
                                selectedOffsets[i] = new Point(selectedNodes[i].Rect.Location.X - mousePos.X, selectedNodes[i].Rect.Location.Y - mousePos.Y);
                            }

                            return;
                        }
                    }
                }
                else
                {
                    Mouse.Capture(this);
                    isMarqueeSelecting = hoveredNode == null;
                    marqueeSelectionStart = mousePos;
                    marqueeSelectionCurrent = mousePos;
                }

                InvalidateVisual();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            MatrixTransform m = GetWorldMatrix();
            Point mousePos = m.Inverse.Transform(e.GetPosition(this));

            if (editingWire != null)
            {
                if (editingWire.HoveredPort != null)
                {
                    BaseNodeVisual.Port sourcePort = (editingWire.Source != null) ? editingWire.SourcePort : editingWire.HoveredPort;
                    BaseNodeVisual.Port targetPort = (editingWire.Target != null) ? editingWire.TargetPort : editingWire.HoveredPort;

                    WireAddedCommand?.Execute(new WireAddedEventArgs(
                        (sourcePort.Owner is NodeVisual) ? (sourcePort.Owner as NodeVisual).Entity : null, 
                        (targetPort.Owner is NodeVisual) ? (targetPort.Owner as NodeVisual).Entity : null, 
                        sourcePort.NameHash, targetPort.NameHash, editingWire.WireType
                        ));
                }

                editingWire.OnMouseUp(mousePos, e.ChangedButton);
                editingWire.Disconnect();
                editingWire = null;

                InvalidateVisual();
                return;
            }

            foreach (BaseVisual visual in visibleNodeVisuals)
            {
                if (visual.OnMouseUp(mousePos, e.ChangedButton))
                {
                    e.Handled = true;
                    InvalidateVisual();
                    return;
                }
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                //prevMousePos = mousePos;
                Mouse.Capture(null);

                if (UndoManager.Instance.PendingUndoUnit != null)
                {
                    // commit any pending moves
                    UndoManager.Instance.CommitUndo(UndoManager.Instance.PendingUndoUnit);
                }

                foreach (WireVisual wire in wireVisuals)
                {
                    if (wire.OnMouseUp(mousePos, e.ChangedButton))
                    {
                        WirePointVisual wirePoint = new WirePointVisual(wire, mousePos.X, mousePos.Y) { UniqueId = Guid.NewGuid() };
                        WireVisual wireToModify = wire;

                        UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add Wire Point",
                            (o) =>
                            {
                                wireToModify.AddWirePoint(wirePoint);
                                AddNode(wirePoint);

                                InvalidateVisual();
                            },
                            (o) =>
                            {
                                wireToModify.RemoveWirePoint(wirePoint);
                                nodeVisuals.Remove(wirePoint);

                                ClearSelection();
                            }));

                        return;
                    }
                }

                if (hoveredNode != null)
                {
                    if (PointEqualsWithTolerance(mousePos, prevMousePos))
                    {
                        if (selectedNodes.Count > 0 && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        {
                            foreach (BaseVisual node in selectedNodes)
                            {
                                node.IsSelected = false;
                            }

                            selectedNodes.Clear();
                            selectedOffsets.Clear();

                            SelectedNodeChangedCommand?.Execute(null);
                        }

                        if (!selectedNodes.Contains(hoveredNode))
                        {
                            selectedNodes.Add(hoveredNode);
                            selectedOffsets.Add(new Point(hoveredNode.Rect.Location.X - mousePos.X, hoveredNode.Rect.Location.Y - mousePos.Y));

                            hoveredNode.IsSelected = true;

                            if (hoveredNode.Data != null)
                            {
                                SelectedNodeChangedCommand?.Execute(hoveredNode.Data);
                            }
                        }
                        InvalidateVisual();
                    }
                }
                else
                {
                    if (!isMarqueeSelecting || PointEqualsWithTolerance(mousePos, prevMousePos))
                    {
                        if (selectedNodes.Count > 0 && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        {
                            foreach (BaseVisual node in selectedNodes)
                            {
                                node.IsSelected = false;
                            }

                            selectedNodes.Clear();
                            selectedOffsets.Clear();

                            SelectedNodeChangedCommand?.Execute(null);
                        }
                    }
                }

                if (isMarqueeSelecting && e.ChangedButton == MouseButton.Left)
                {
                    isMarqueeSelecting = false;
                    Mouse.Capture(null);

                    InvalidateVisual();
                    return;
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        protected override void Render(DrawingContext drawingContext)
        {
            if (ItemsSource == null)
                return;

            UpdateVisibleNodes();

            if (ItemsSource.World.IsSimulationRunning)
            {
                DrawingContext drawingContext2 = debugLayerVisual.RenderOpen();
                RenderDebug(drawingContext2);
                drawingContext2.Close();

                drawingContext2 = debugWireVisual.RenderOpen();
                RenderDebugWires(drawingContext2);
                drawingContext2.Close();
            }

            DrawingContextState state = new DrawingContextState(this, drawingContext);
            {
                // draw wires
                for (int i = 0; i < wireVisuals.Count; i++)
                {
                    wireVisuals[i].Render(state);
                }
                if (editingWire != null)
                {
                    editingWire.Render(state);
                }

                if (ItemsSource.World.IsSimulationRunning)
                {
                    drawingContext.DrawDrawing(debugWireVisual.Drawing);
                }

                // draw nodes
                for (int i = visibleNodeVisuals.Count - 1; i >= 0; i--)
                {
                    visibleNodeVisuals[i].Render(state);
                }

                if (ItemsSource.World.IsSimulationRunning)
                {
                    drawingContext.DrawDrawing(debugLayerVisual.Drawing);
                }

                DrawSelectionRect(drawingContext);
            }
        }

        protected void RenderDebugWires(DrawingContext drawingContext)
        {
            DrawingContextState state = new DrawingContextState(this, drawingContext);

            for (int i = 0; i < wireVisuals.Count; i++)
            {
                wireVisuals[i].RenderDebug(state);
            }
        }

        protected void RenderDebug(DrawingContext drawingContext)
        {
            DrawingContextState state = new DrawingContextState(this, drawingContext);

            for (int i = visibleNodeVisuals.Count - 1; i >= 0; i--)
            {
                visibleNodeVisuals[i].RenderDebug(state);
            }

            drawingContext.PushOpacity(0.25);
            drawingContext.DrawText(simulationText, new Point(ActualWidth - simulationText.Width - 10, ActualHeight - simulationText.Height));
            drawingContext.Pop();
        }

        protected override void UpdateScaleParameters()
        {
            base.UpdateScaleParameters();

            largeFont.Update(scale);
            smallFont.Update(scale);
        }

        private void ClearSelection()
        {
            foreach (BaseVisual node in selectedNodes)
            {
                node.IsSelected = false;
            }
            selectedNodes.Clear();
            selectedOffsets.Clear();

            InvalidateVisual();
        }

        private void AddNode(BaseVisual nodeToAdd)
        {
            foreach (BaseVisual node in nodeVisuals.Where(n => n is CommentNodeVisual))
            {
                if (node.Rect.Contains(nodeToAdd.Rect))
                {
                    CommentNodeVisual commentNode = node as CommentNodeVisual;
                    commentNode.AddNode(nodeToAdd);
                }
            }
            nodeVisuals.Add(nodeToAdd);
        }

        private void RemoveWire(WireVisual wire)
        {
            wire.Disconnect();
            wireVisuals.Remove(wire);
        }

        private void DrawSelectionRect(DrawingContext drawingContext)
        {
            if (!isMarqueeSelecting)
                return;

            MatrixTransform m = GetWorldMatrix();

            double width = marqueeSelectionCurrent.X - marqueeSelectionStart.X;
            double height = marqueeSelectionCurrent.Y - marqueeSelectionStart.Y;

            if (width == 0 && height == 0)
                return;

            Point startPos = marqueeSelectionStart;

            if (width <= 0)
            {
                width = marqueeSelectionStart.X - marqueeSelectionCurrent.X;
                startPos.X = marqueeSelectionCurrent.X;
            }
            if (height <= 0)
            {
                height = marqueeSelectionStart.Y - marqueeSelectionCurrent.Y;
                startPos.Y = marqueeSelectionCurrent.Y;
            }

            drawingContext.DrawRectangle(null, marqueePen, new Rect(m.Transform(startPos), new Size(width * scale, height * scale)));
        }

        private void UpdateVisibleNodes()
        {
            MatrixTransform m = GetWorldMatrix();
            Point topLeft = m.Inverse.Transform(new Point(0, 0));
            Point bottomRight = m.Inverse.Transform(new Point(ActualWidth, ActualHeight));
            Rect fustrum = new Rect(topLeft, bottomRight);

            visibleNodeVisuals.Clear();
            foreach (BaseVisual visual in nodeVisuals)
            {
                if (fustrum.IntersectsWith(visual.Rect) || visual.IsSelected)
                {
                    if (visual.IsSelected) visibleNodeVisuals.Insert(0, visual);
                    else visibleNodeVisuals.Add(visual);
                }
            }
        }

        private void GenerateNodes(IEnumerable<Entity> entities)
        {
            ContextMenu portContextMenu = new ContextMenu();
            MenuItem makeShortcutItem = new MenuItem() { Header = "Make shortcut" };
            makeShortcutItem.Click += (o, e) =>
            {
                BaseNodeVisual.Port port = (o as MenuItem).DataContext as BaseNodeVisual.Port;
                GenerateShortcuts(port.Owner, port);
            };
            portContextMenu.Items.Add(makeShortcutItem);

            Random r = new Random(2);
            foreach (Entity entity in entities)
            {
                ILogicEntity logicEntity = entity as ILogicEntity;
                ContextMenu nodeContextMenu = new ContextMenu();

                if (logicEntity is LogicEntity)
                {
                    LogicEntity l = logicEntity as LogicEntity;
                    foreach (ConnectionDesc conn in l.Events)
                    {
                        if (conn.IsTriggerable)
                        {
                            MenuItem triggerMenuItem = new MenuItem() { Header = $"Trigger {conn.Name}" };
                            triggerMenuItem.Click += (o, e) => { l.EventToTrigger = Frosty.Hash.Fnv1.HashString(conn.Name); };
                            nodeContextMenu.Items.Add(triggerMenuItem);
                        }
                    }
                }

                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;
                bool isCollapsed = entity.UserData == null;

                x = x - (x % 10);
                y = y - (y % 10);

                NodeVisual node = new NodeVisual(logicEntity, x, y) 
                { 
                    GlyphWidth = OriginalAdvanceWidth, 
                    NodeContextMenu = (nodeContextMenu.Items.Count > 0) ? nodeContextMenu : null,
                    PortContextMenu = portContextMenu, 
                    UniqueId = Guid.NewGuid() 
                };

                AddNode(node);

                node.IsCollapsed = isCollapsed;
                node.Update();

                if (entity.UserData != null && entity.UserData is Point)
                {
                    Point mousePos = GetWorldMatrix().Inverse.Transform((Point)entity.UserData);

                    x = mousePos.X;
                    y = mousePos.Y;

                    node.IsCollapsed = false;
                    node.Rect.X = x - node.Rect.Width * 0.5;
                    node.Rect.Y = y - node.Rect.Height * 0.5;
                }
            }
        }

        private void GenerateInterface()
        {
            if (ItemsSource.InterfaceDescriptor == null)
                return;

            InterfaceDescriptor interfaceDesc = ItemsSource.InterfaceDescriptor as InterfaceDescriptor;
            Random r = new Random(1);

            foreach (DataField dataField in interfaceDesc.Data.Fields)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                switch (dataField.AccessType)
                {
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_Source: nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() }); break;
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_Target: nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() }); break;
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_SourceAndTarget:
                        nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth });
                        nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth });
                        break;
                }
            }

            foreach (DynamicEvent eventField in interfaceDesc.Data.InputEvents)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, eventField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
            foreach (DynamicEvent eventField in interfaceDesc.Data.OutputEvents)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, eventField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }

            foreach (DynamicLink linkField in interfaceDesc.Data.InputLinks)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, linkField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
            foreach (DynamicLink linkField in interfaceDesc.Data.OutputLinks)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, linkField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
        }

        private void GenerateWires(IEnumerable<object> connections, int connectionType)
        {
            IEnumerable<BaseNodeVisual> nodes = nodeVisuals.Where(n => n is BaseNodeVisual).Select(n => n as BaseNodeVisual);
            foreach (object obj in connections)
            {
                FrostySdk.Ebx.PointerRef source = new FrostySdk.Ebx.PointerRef();
                FrostySdk.Ebx.PointerRef target = new FrostySdk.Ebx.PointerRef();
                int sourceId = -1;
                int targetId = -1;
                string sourceName = null;
                string targetName = null;

                switch (connectionType)
                {
                    case 0:
                        {
                            LinkConnection linkConnection = obj as FrostySdk.Ebx.LinkConnection;
                            source = MakeRef(linkConnection.Source, ItemsSource.BlueprintGuid);
                            target = MakeRef(linkConnection.Target, ItemsSource.BlueprintGuid);
                            sourceId = linkConnection.SourceFieldId;
                            targetId = linkConnection.TargetFieldId;
                            sourceName = linkConnection.SourceField;
                            targetName = linkConnection.TargetField;
                        }
                        break;
                    case 1:
                        {
                            EventConnection eventConnection = obj as FrostySdk.Ebx.EventConnection;
                            source = MakeRef(eventConnection.Source, ItemsSource.BlueprintGuid);
                            target = MakeRef(eventConnection.Target, ItemsSource.BlueprintGuid);
                            sourceId = eventConnection.SourceEvent.Id;
                            targetId = eventConnection.TargetEvent.Id;
                            sourceName = eventConnection.SourceEvent.Name;
                            targetName = eventConnection.TargetEvent.Name;
                        }
                        break;
                    case 2:
                        {
                            PropertyConnection propertyConnection = obj as FrostySdk.Ebx.PropertyConnection;
                            source = MakeRef(propertyConnection.Source, ItemsSource.BlueprintGuid);
                            target = MakeRef(propertyConnection.Target, ItemsSource.BlueprintGuid);
                            sourceId = propertyConnection.SourceFieldId;
                            targetId = propertyConnection.TargetFieldId;
                            sourceName = propertyConnection.SourceField;
                            targetName = propertyConnection.TargetField;
                        }
                        break;
                }

                BaseVisual sourceVisual = nodes.FirstOrDefault(v => v.Matches(source, sourceId, 0));
                BaseVisual targetVisual = nodes.FirstOrDefault(v => v.Matches(target, targetId, 1));

                if (sourceVisual == null || targetVisual == null)
                    continue;

                WireVisual wire = new WireVisual(
                    sourceVisual as BaseNodeVisual,
                    sourceName,
                    sourceId,
                    targetVisual as BaseNodeVisual,
                    targetName,
                    targetId,
                    connectionType);
                wire.Data = obj;

                if (wire.SourcePort == null || wire.TargetPort == null)
                    continue;

                wireVisuals.Add(wire);
            }
        }

        private void Entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateNodes(e.NewItems.OfType<Entity>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object entity in e.OldItems)
                {
                    BaseVisual visual = nodeVisuals.Find(n => n.Data == entity);
                    
                    nodeVisuals.Remove(visual);
                    
                    // remove wires
                    if (visual is NodeVisual node)
                    {
                        foreach (NodeVisual.Port port in node.AllPorts)
                        {
                            List<WireVisual> wiresToRemove = new List<WireVisual>();
                            foreach (WireVisual wire in port.Connections)
                            {
                                wiresToRemove.Add(wire);
                            }
                            
                            foreach (WireVisual wire in wiresToRemove)
                            {
                                RemoveWire(wire);
                            }
                        }
                    }
                }
            }

            InvalidateVisual();
        }

        private void LinkConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateWires(e.NewItems.OfType<object>(), 0);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object connection in e.OldItems)
                {
                    WireVisual wire = wireVisuals.Find(n => n.Data == connection);
                    RemoveWire(wire);
                }
            }

            InvalidateVisual();
        }

        private void EventConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateWires(e.NewItems.OfType<object>(), 1);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object connection in e.OldItems)
                {
                    WireVisual wire = wireVisuals.Find(n => n.Data == connection);
                    RemoveWire(wire);
                }
            }

            InvalidateVisual();
        }

        private void PropertyConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateWires(e.NewItems.OfType<object>(), 2);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object connection in e.OldItems)
                {
                    WireVisual wire = wireVisuals.Find(n => n.Data == connection);
                    RemoveWire(wire);
                }
            }

            InvalidateVisual();
        }

        private FrostySdk.Ebx.PointerRef MakeRef(FrostySdk.Ebx.PointerRef pr, Guid guid)
        {
            if (pr.Type == PointerRefType.External)
                return pr;

            return new FrostySdk.Ebx.PointerRef(new EbxImportReference() { FileGuid = guid, ClassGuid = pr.GetInstanceGuid() });
        }

        private bool PointEqualsWithTolerance(Point a, Point b)
        {
            return a.X >= b.X - 0.5 && a.Y >= b.Y - 0.5 && a.X <= b.X + 0.5 && a.Y <= b.Y + 0.5;
        }

        private static void OnEntitiesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchematicsCanvas canvas = o as SchematicsCanvas;
            canvas.Update();
        }

        private static void OnDataModified(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            SchematicsCanvas canvas = o as SchematicsCanvas;
            foreach (BaseVisual node in canvas.visibleNodeVisuals)
                node.Update();
            canvas.InvalidateVisual();
        }

        private static void OnDebugUpdateModified(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            SchematicsCanvas canvas = o as SchematicsCanvas;
            canvas.UpdateDebugLayer();
        }
    }
}
