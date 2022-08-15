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
using LevelEditorPlugin.Library.Schematics;
using LevelEditorPlugin.Windows;
using Entity = LevelEditorPlugin.Entities.Entity;

namespace LevelEditorPlugin.Controls
{
    public class SchematicsData
    {
        public ObservableCollection<Entity> Entities;
        public ObservableCollection<object> LinkConnections;
        public ObservableCollection<object> EventConnections;
        public ObservableCollection<object> PropertyConnections;
        public object InterfaceDescriptor;
        public Guid BlueprintGuid;
        public string BlueprintName;
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

    public class WireRemovedEventArgs : EventArgs
    {
        public object Connection { get; private set; }

        public ILogicEntity SourceEntity { get; private set; }
        public ILogicEntity TargetEntity { get; private set; }
        
        public UndoContainer Container { get; private set; }
        
        public WireRemovedEventArgs(object inConnection, ILogicEntity inSource, ILogicEntity inTarget, UndoContainer inContainer)
        {
            Connection = inConnection;

            SourceEntity = inSource;
            TargetEntity = inTarget;

            Container = inContainer;
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
        #region -- Internal Classes --
        
        public class DrawingContextState
        {
            public DrawingContext DrawingContext { get; private set; }

            // Options
            public bool ConnectorOrdersVisible { get; private set; }
            public bool UseCurvedLines { get; private set; }
            public bool UseAlternateConnectorOrders { get; private set; }

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
            public Pen InnerPortPen { get; private set; }
            public Pen GridMinorPen { get; private set; }
            public Pen GridMajorPen { get; private set; }
            public Pen WireLinkPen { get; private set; }
            public Pen WireEventPen { get; private set; }
            public Pen WirePropertyPen { get; private set; }
            public Pen WireHitTestPen { get; private set; }
            public Pen ShortcutWirePen { get; private set; }
            public Pen WireSelectedPen { get; private set; }
            public Pen WireDeletingPen { get; private set; }
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
                UseCurvedLines = owner.UseCurvedLines;
                UseAlternateConnectorOrders = owner.UseAlternateConnectorOrders;

                BlackPen = owner.blackPen;
                GridMinorPen = owner.gridMinorPen;
                GridMajorPen = owner.gridMajorPen;
                WireLinkPen = owner.m_wireLinkPen;
                WireEventPen = owner.m_wireEventPen;
                WirePropertyPen = owner.m_wirePropertyPen;
                WireHitTestPen = owner.m_wireHitTestPen;
                ShortcutWirePen = owner.m_shortcutWirePen;
                WireSelectedPen = owner.m_wireSelectedPen;
                WireDeletingPen = owner.m_wireDeletingPen;
                GlowPen = owner.m_glowPen;
                InnerPortPen = new Pen(Brushes.Black, 1.0d * Scale);

                NodeTitleBackgroundBrush = new SolidColorBrush(Color.FromRgb(63, 63, 63));
                NodeBackgroundBrush = new SolidColorBrush(Color.FromRgb(194, 194, 194));
                NodeSelectedBrush = Brushes.PaleGoldenrod;
                NodeCollapseButtonBackgroundBrush = new SolidColorBrush(Color.FromRgb(248, 248, 248));
                
                SchematicLinkBrush = owner.SchematicLinkBrush;
                SchematicEventBrush = owner.SchematicEventBrush;
                SchematicPropertyBrush = owner.SchematicPropertyBrush;
                SchematicRealmEnabled = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                SchematicRealmDisabled = new SolidColorBrush(Color.FromRgb(155, 155, 155));

                LastFrameCount = owner.m_lastFrameCount;

                LargeFont = owner.m_largeFont;
                SmallFont = owner.m_smallFont;
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
        
        #endregion

        #region -- Properties --

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(SchematicsData), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null, OnEntitiesChanged));
        public static readonly DependencyProperty ConnectorOrdersVisibleProperty = DependencyProperty.Register("ConnectorOrdersVisible", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty UseCurvedLinesProperty = DependencyProperty.Register("UseCurvedLines", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty UseAlternateConnectorOrdersProperty = DependencyProperty.Register("UseAlternateConnectorOrders", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty SuppressLayoutSaveProperty = DependencyProperty.Register("SuppressLayoutSave", typeof(bool), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty SchematicLinkBrushProperty = DependencyProperty.Register("SchematicLinkBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SchematicEventBrushProperty = DependencyProperty.Register("SchematicEventBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SchematicPropertyBrushProperty = DependencyProperty.Register("SchematicPropertyBrush", typeof(Brush), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SelectedNodeChangedCommandProperty = DependencyProperty.Register("SelectedNodeChangedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty WireAddedCommandProperty = DependencyProperty.Register("WireAddedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty WireRemovedCommandProperty = DependencyProperty.Register("WireRemovedCommand", typeof(ICommand), typeof(SchematicsCanvas), new FrameworkPropertyMetadata(null));
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
        public bool UseCurvedLines
        {
            get => (bool)GetValue(UseCurvedLinesProperty);
            set => SetValue(UseCurvedLinesProperty, value);
        }
        public bool UseAlternateConnectorOrders
        {
            get => (bool)GetValue(UseAlternateConnectorOrdersProperty);
            set => SetValue(UseAlternateConnectorOrdersProperty, value);
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
        public ICommand WireRemovedCommand
        {
            get => (ICommand)GetValue(WireRemovedCommandProperty);
            set => SetValue(WireRemovedCommandProperty, value);
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
        
        #endregion

        #region -- Members --
        
        public object UpdateDebug
        {
            get => GetValue(UpdateDebugProperty);
            set => SetValue(UpdateDebugProperty, value);
        }

        public double OriginalAdvanceWidth;

        private FontData m_largeFont;
        private FontData m_smallFont;

        private Pen m_wireLinkPen;
        private Pen m_wireEventPen;
        private Pen m_wirePropertyPen;
        private Pen m_wireHitTestPen;
        private Pen m_shortcutWirePen;
        private Pen m_wireSelectedPen;
        private Pen m_wireDeletingPen;
        private Pen m_marqueePen;
        private Pen m_glowPen;

        private List<BaseVisual> m_nodeVisuals = new List<BaseVisual>();
        private List<BaseVisual> m_visibleNodeVisuals = new List<BaseVisual>();
        private List<WireVisual> m_wireVisuals = new List<WireVisual>();

        private List<BaseVisual> m_selectedNodes = new List<BaseVisual>();
        private List<Point> m_selectedOffsets = new List<Point>();

        private BaseVisual m_hoveredNode;

        private bool m_isMouseDown;
        private bool m_isMouseMoving;
        
        private bool m_isMarqueeSelecting;
        private Point m_marqueeSelectionStart;
        private Point m_marqueeSelectionCurrent;

        private bool m_isCuttingWire;
        private Point m_wireCutStart;
        private Point m_wireCutCurrent;
        private List<WireVisual> m_wiresToCut = new List<WireVisual>();
        
        private FormattedText m_simulationText;

        private DrawingVisual m_debugLayerVisual;
        private DrawingVisual m_debugWireVisual;

        private long m_lastFrameCount = -1;
        private WireVisual m_editingWire = null;

        #endregion

        #region -- Constructors --

        public SchematicsCanvas()
        {
            m_largeFont = FontData.MakeFont(new Typeface("Consolas"), 10);
            m_smallFont = FontData.MakeFont(new Typeface("Consolas"), 7);
            OriginalAdvanceWidth = m_largeFont.AdvanceWidth;

            viewport = new TranslateTransform();

            Focusable = true;
            IsEnabled = true;

            m_simulationText = new FormattedText("SIMULATING", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 48.0, Brushes.White, 96.0);

            m_debugLayerVisual = new DrawingVisual();
            m_debugWireVisual = new DrawingVisual();
        }

        #endregion

        #region -- Override Methods --
        
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            m_wireLinkPen = new Pen(SchematicLinkBrush, 2.0); m_wireLinkPen.Freeze();
            m_wireEventPen = new Pen(SchematicEventBrush, 2.0); m_wireEventPen.Freeze();
            m_wirePropertyPen = new Pen(SchematicPropertyBrush, 2.0); m_wirePropertyPen.Freeze();
            m_wireHitTestPen = new Pen(Brushes.Transparent, 4.0); m_wireHitTestPen.Freeze();
            m_shortcutWirePen = new Pen(Brushes.WhiteSmoke, 2.0) { DashStyle = DashStyles.Dash }; m_shortcutWirePen.Freeze();
            m_wireSelectedPen = new Pen(Brushes.PaleGoldenrod, 2.0); m_wireSelectedPen.Freeze();
            m_wireDeletingPen = new Pen(Brushes.Red, 2.0); m_wireDeletingPen.Freeze();
            m_marqueePen = new Pen(Brushes.White, 2.0) { DashStyle = DashStyles.Dash }; m_marqueePen.Freeze();
            m_glowPen = new Pen(Brushes.Yellow, 5.0); m_glowPen.Freeze();

            Unloaded += (o, ev) =>
            {
                if (SuppressLayoutSave)
                    return;

                SchematicsLayout layout = new SchematicsLayout()
                {
                    BlueprintName = ItemsSource.BlueprintName,
                    FileGuid = ItemsSource.BlueprintGuid,
                    CanvasPosition = new Point(offset.X, offset.Y),
                    CanvasScale = scale,
                    Nodes = new List<SchematicsLayout.Node>(),
                    Wires = new List<SchematicsLayout.Wire>(),
                    Comments = new List<SchematicsLayout.Comment>()
                };

                foreach (BaseVisual node in m_nodeVisuals)
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

                for (int i = 0; i < m_wireVisuals.Count; i++)
                {
                    if (m_wireVisuals[i].WirePoints.Count > 0)
                    {
                        SchematicsLayout.Wire wireLayout = m_wireVisuals[i].GenerateLayout();
                        wireLayout.Index = i;

                        layout.Wires.Add(wireLayout);
                    }
                }

                SchematicsLayoutManager.Instance.AddLayout(ItemsSource.BlueprintGuid, layout);
            };

            Focus();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            // create shortcut
            if (m_selectedNodes.Count == 1 && m_selectedNodes[0] is InterfaceNodeVisual && !(m_selectedNodes[0] is InterfaceShortcutNodeVisual))
            {
                if (e.Key == Key.S)
                {
                    InterfaceNodeVisual node = m_selectedNodes[0] as InterfaceNodeVisual;
                    if (node.Self.ShortcutNode == null)
                    {
                        GenerateShortcuts(node, node.Self);
                    }

                    e.Handled = true;
                }
            }
            // delete node(s)
            else if (m_selectedNodes.Count > 0 && (e.Key == Key.Delete || e.Key == Key.X))
            {
                UndoContainer container = new UndoContainer("Delete Node(s)");
                for (int i = m_selectedNodes.Count - 1; i >= 0; i--)
                {
                    BaseVisual node = m_selectedNodes[i];

                    // wire point
                    if (node is WirePointVisual wirePoint)
                    {
                        container.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                wirePoint.Wire.RemoveWirePoint(wirePoint);
                                m_nodeVisuals.Remove(wirePoint);
                            },
                            (o) =>
                            {
                                wirePoint.Wire.AddWirePoint(wirePoint, shouldForceAdd: true);
                                m_nodeVisuals.Add(wirePoint);
                            }));

                        m_selectedNodes.RemoveAt(i);
                        m_selectedOffsets.RemoveAt(i);
                    }

                    // comment
                    if (node is CommentNodeVisual comment)
                    {
                        container.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                m_nodeVisuals.Remove(comment);
                            },
                            (o) =>
                            {
                                m_nodeVisuals.Add(comment);
                            }));

                        m_selectedNodes.RemoveAt(i);
                        m_selectedOffsets.RemoveAt(i);
                    }

                    // node
                    if (node is NodeVisual entity)
                    {
                        NodeRemovedCommand?.Execute(new NodeRemovedEventArgs(entity.Entity, container));
                    }

                    // shortcut
                    if (node is InterfaceShortcutNodeVisual shortcut)
                    {
                        List<InterfaceShortcutNodeVisual> shortcutsToRemove = new List<InterfaceShortcutNodeVisual>();
                        List<WireVisual> wiresToAdd = new List<WireVisual>();
                        List<WireVisual> wiresToRemove = new List<WireVisual>();
                        InterfaceShortcutNodeVisual owner = null;
                        if (shortcut == shortcut.OwnerPort.ShortcutNode)
                        {
                            owner = shortcut;

                            WireVisual wireToRemove = m_wireVisuals.Find(w => shortcut.OwnerPort.PortDirection == 0 ? w.Target == shortcut : w.Source == shortcut);

                            if (wireToRemove != null)
                            {
                                wiresToRemove.Add(wireToRemove);
                            }

                            foreach (InterfaceShortcutNodeVisual childShortcut in shortcut.OwnerPort.ShortcutChildren)
                            {
                                shortcutsToRemove.Add(childShortcut);
                                WireVisual childWireToRemove = m_wireVisuals.Find(w => childShortcut.OwnerPort.PortDirection == 0 ? w.Source == childShortcut : w.Target == childShortcut);
                                if (childWireToRemove != null)
                                {
                                    wiresToRemove.Add(childWireToRemove);

                                    BaseNodeVisual source = shortcut.OwnerPort.PortDirection == 0 ? childShortcut.OwnerPort.Owner : childWireToRemove.Source;
                                    BaseNodeVisual.Port sourcePort = (shortcut.OwnerPort.PortDirection == 0) ? childShortcut.OwnerPort : childWireToRemove.SourcePort;
                                    BaseNodeVisual target = shortcut.OwnerPort.PortDirection == 0 ? childWireToRemove.Target : childShortcut.OwnerPort.Owner;
                                    BaseNodeVisual.Port targetPort = (shortcut.OwnerPort.PortDirection == 0) ? childWireToRemove.TargetPort : childShortcut.OwnerPort;

                                    wiresToAdd.Add(new WireVisual(source, sourcePort.Name, sourcePort.NameHash, target, targetPort.Name, targetPort.NameHash, childWireToRemove.WireType));
                                }
                            }
                        }
                        else
                        {
                            shortcutsToRemove.Add(shortcut);

                            WireVisual wireToRemove = m_wireVisuals.Find(w => shortcut.OwnerPort.PortDirection == 0 ? w.Source == shortcut : w.Target == shortcut);

                            if (wireToRemove != null)
                            {
                                wiresToRemove.Add(wireToRemove);

                                BaseNodeVisual source = shortcut.OwnerPort.PortDirection == 0 ? shortcut.OwnerPort.Owner : wireToRemove.Source;
                                BaseNodeVisual.Port sourcePort = (shortcut.OwnerPort.PortDirection == 0) ? shortcut.OwnerPort : wireToRemove.SourcePort;
                                BaseNodeVisual target = shortcut.OwnerPort.PortDirection == 0 ? wireToRemove.Target : shortcut.OwnerPort.Owner;
                                BaseNodeVisual.Port targetPort = (shortcut.OwnerPort.PortDirection == 0) ? wireToRemove.TargetPort : shortcut.OwnerPort;

                                wiresToAdd.Add(new WireVisual(source, sourcePort.Name, sourcePort.NameHash, target, targetPort.Name, targetPort.NameHash, wireToRemove.WireType));
                            }

                            if (shortcut.OwnerPort.ShortcutChildren.Count == 1)
                            {
                                owner = shortcut.OwnerPort.ShortcutNode;
                                wireToRemove = m_wireVisuals.Find(w => shortcut.OwnerPort.PortDirection == 0 ? w.Target == shortcut.OwnerPort.ShortcutNode : w.Source == shortcut.OwnerPort.ShortcutNode);
                                if (wireToRemove != null)
                                {
                                    wiresToRemove.Add(wireToRemove);
                                }
                            }
                        }

                        container.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                foreach (InterfaceShortcutNodeVisual shortcutToRemove in shortcutsToRemove)
                                {
                                    shortcut.OwnerPort.ShortcutChildren.Remove(shortcutToRemove);
                                    m_nodeVisuals.Remove(shortcutToRemove);
                                }
                                if (owner != null)
                                {
                                    shortcut.OwnerPort.ShortcutNode = null;
                                    m_nodeVisuals.Remove(owner);
                                }
                                foreach (WireVisual wireToRemove in wiresToRemove)
                                {
                                    foreach (WirePointVisual wirePointToRemove in wireToRemove.WirePoints)
                                    {
                                        m_nodeVisuals.Remove(wirePointToRemove);
                                    }
                                    m_wireVisuals.Remove(wireToRemove);
                                }
                                foreach (WireVisual wireToAdd in wiresToAdd)
                                {
                                    m_wireVisuals.Add(wireToAdd);
                                }
                            },
                            (o) =>
                            {
                                foreach (InterfaceShortcutNodeVisual shortcutToRemove in shortcutsToRemove)
                                {
                                    shortcut.OwnerPort.ShortcutChildren.Add(shortcutToRemove);
                                    m_nodeVisuals.Add(shortcutToRemove);
                                }
                                if (owner != null)
                                {
                                    shortcut.OwnerPort.ShortcutNode = owner;
                                    m_nodeVisuals.Add(owner);
                                }
                                foreach (WireVisual wireToRemove in wiresToRemove)
                                {
                                    foreach (WirePointVisual wirePointToRemove in wireToRemove.WirePoints)
                                    {
                                        m_nodeVisuals.Add(wirePointToRemove);
                                    }
                                    m_wireVisuals.Add(wireToRemove);
                                }
                                foreach (WireVisual wireToAdd in wiresToAdd)
                                {
                                    m_wireVisuals.Remove(wireToAdd);
                                }
                            }));

                        m_selectedNodes.RemoveAt(i);
                        m_selectedOffsets.RemoveAt(i);
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
                if (m_selectedNodes.Count > 0)
                {
                    CommentNodeVisual commentNode = new CommentNodeVisual(new CommentNodeData() { Color = new FrostySdk.Ebx.Vec4() { x = 0.25f, y = 0.0f, z = 0.0f, w = 1.0f }, CommentText = "@todo" }, m_selectedNodes, 0, 0) { UniqueId = Guid.NewGuid() };
                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Create Comment Group",
                        (o) =>
                        {
                            AddNode(commentNode);
                            InvalidateVisual();
                        },
                        (o) =>
                        {
                            m_nodeVisuals.Remove(commentNode);
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
            if (m_isMouseDown)
            {
                m_isMouseMoving = true;
            }
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Focus();

                MatrixTransform m = GetWorldMatrix();
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                // update editing wire
                if (m_editingWire != null)
                {
                    m_editingWire.OnMouseMove(mousePos);
                    m_editingWire.UpdateEditing(mousePos, m_visibleNodeVisuals.Where(n => n is BaseNodeVisual));
                    InvalidateVisual();
                    return;
                }

                // only move full node if collapse button or highlighted port isn't hovered
                if (m_hoveredNode != null && m_hoveredNode is NodeVisual)
                {
                    if ((m_hoveredNode as NodeVisual).IsCollapseButtonHovered)
                    {
                        return;
                    }
                    if ((m_hoveredNode as NodeVisual).HighlightedPort != null)
                    {
                        return;
                    }
                }

                // move nodes
                if (m_selectedNodes.Count > 0 && !m_isMarqueeSelecting && !m_isCuttingWire)
                {
                    if (!UndoManager.Instance.IsUndoing && UndoManager.Instance.PendingUndoUnit == null)
                    {
                        List<BaseVisual> prevSelection = new List<BaseVisual>();
                        prevSelection.AddRange(m_selectedNodes);

                        List<Point> prevPositions = new List<Point>();
                        foreach (BaseVisual node in m_selectedNodes)
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

                    for (int i = 0; i < m_selectedNodes.Count; i++)
                    {
                        double newX = mousePos.X + m_selectedOffsets[i].X;
                        double newY = mousePos.Y + m_selectedOffsets[i].Y;

                        newX -= (newX % 5);
                        newY -= (newY % 5);

                        m_selectedNodes[i].Move(new Point(newX, newY));
                    }
                    InvalidateVisual();
                    return;
                }

                if (m_isMarqueeSelecting)
                {
                    m_marqueeSelectionCurrent = mousePos;

                    double width = m_marqueeSelectionCurrent.X - m_marqueeSelectionStart.X;
                    double height = m_marqueeSelectionCurrent.Y - m_marqueeSelectionStart.Y;
                    Point startPos = m_marqueeSelectionStart;

                    if (width <= 0)
                    {
                        width = m_marqueeSelectionStart.X - m_marqueeSelectionCurrent.X;
                        startPos.X = m_marqueeSelectionCurrent.X;
                    }
                    if (height <= 0)
                    {
                        height = m_marqueeSelectionStart.Y - m_marqueeSelectionCurrent.Y;
                        startPos.Y = m_marqueeSelectionCurrent.Y;
                    }

                    Rect selectionRect = new Rect(startPos, new Size(width, height));

                    for (int i = m_selectedNodes.Count - 1; i >= 0; i--)
                    {
                        if (!selectionRect.IntersectsWith(m_selectedNodes[i].Rect))
                        {
                            m_selectedNodes[i].IsSelected = false;
                            m_selectedNodes.RemoveAt(i);
                            m_selectedOffsets.RemoveAt(i);
                        }
                    }

                    foreach (BaseVisual node in m_nodeVisuals)
                    {
                        if (selectionRect.IntersectsWith(node.Rect))
                        {
                            if (!m_selectedNodes.Contains(node))
                            {
                                node.IsSelected = true;
                                m_selectedNodes.Add(node);
                                m_selectedOffsets.Add(new Point(node.Rect.Location.X - mousePos.X, node.Rect.Location.Y - mousePos.Y));
                            }
                        }
                    }

                    InvalidateVisual();
                }
                else if (m_isCuttingWire)
                {
                    m_wireCutCurrent = mousePos;
                    MatrixTransform mat = GetWorldMatrix();

                    // clear list to avoid cutting wires that were previously hovered over
                    m_wiresToCut.Clear();
                    foreach (WireVisual wire in m_wireVisuals)
                    {
                        LineGeometry wireCutLine = new LineGeometry();
                        bool isCut = false;
                        for (int i = 0; i < wire.WireGeometry.Children.Count; i++)
                        {
                            wireCutLine = wire.WireGeometry.Children[i] as LineGeometry;
                            if (Intersect(m_wireCutStart, m_wireCutCurrent, wireCutLine.StartPoint, wireCutLine.EndPoint))
                            {
                                m_wiresToCut.Add(wire);
                                wire.IsMarkedForDeletion = true; isCut = true;
                                break;
                            }
                        }
                        if (isCut)
                        {
                            continue;
                        }
                        wire.IsMarkedForDeletion = false;
                    }

                    InvalidateVisual();
                }
            }
            else
            {
                MatrixTransform m = GetWorldMatrix();
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                bool hasEnteredHoverState = false;
                foreach (BaseVisual visual in m_visibleNodeVisuals)
                {
                    if (visual.Bounds.Contains(mousePos) && visual.HitTest(mousePos))
                    {
                        bool invalidate = false;
                        if (m_hoveredNode != null && m_hoveredNode != visual)
                        {
                            m_hoveredNode.OnMouseLeave();
                            invalidate = true;
                        }

                        m_hoveredNode = visual;

                        if (visual.OnMouseOver(mousePos))
                        {
                            invalidate = true;
                        }

                        if (invalidate)
                        {
                            InvalidateVisual();
                        }

                        hasEnteredHoverState = true;
                    }
                }

                if (m_hoveredNode != null && !hasEnteredHoverState)
                {
                    if (m_hoveredNode.OnMouseLeave())
                        InvalidateVisual();

                    m_hoveredNode = null;
                    Mouse.OverrideCursor = null;
                }
            }

            base.OnMouseMove(e);

            bool Ccw(Point a, Point b, Point c)
            {
                return (c.Y - a.Y) * (b.X - a.X) > (b.Y - a.Y) * (c.X - a.X);
            }
    
            bool Intersect(Point start1, Point end1, Point start2, Point end2)
            {
                return Ccw(start1, start2, end2) != Ccw(end1, start2, end2) && Ccw(start1, end1, start2) != Ccw(start1, end1, end2);
            }
    
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            m_isMouseDown = true;
            
            MatrixTransform m = GetWorldMatrix();
            Point mousePos = m.Inverse.Transform(e.GetPosition(this));

            foreach (BaseVisual visual in m_visibleNodeVisuals)
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

                if (m_hoveredNode != null)
                {
                    if (m_hoveredNode is BaseNodeVisual && (m_hoveredNode as BaseNodeVisual).HighlightedPort != null)
                    {
                        BaseNodeVisual node = m_hoveredNode as BaseNodeVisual;
                        
                        // remove connections attached to port if holding Left Alt, if not start editing wire
                        if (Keyboard.IsKeyDown(Key.LeftAlt))
                        {
                            UndoContainer container = new UndoContainer("Delete Connection(s)");
                            
                            BaseNodeVisual.Port port = node.HighlightedPort;
                            for (int i = port.Connections.Count - 1; i >= 0; i--)
                            {
                                WireVisual wire = port.Connections[i];
                                
                                WireRemovedCommand?.Execute(new WireRemovedEventArgs(wire.Data,
                                    (wire.Source is NodeVisual) ? (wire.Source as NodeVisual).Entity : null, 
                                    (wire.Target is NodeVisual) ? (wire.Target as NodeVisual).Entity : null, container));
                                
                                // update nodes to ensure sizing is correct
                                wire.Source.Update();
                                wire.Target.Update();
                            }
                            
                            if (container.HasItems)
                            {
                                container.Add(new GenericUndoUnit("", o => InvalidateVisual(), o => {}));
                                UndoManager.Instance.CommitUndo(container);
                            }
                        }
                        else
                        {
                            BaseNodeVisual sourceNode = (node.HighlightedPort.PortDirection == 0) ? node : null;
                            BaseNodeVisual targetNode = (node.HighlightedPort.PortDirection == 1) ? node : null;

                            m_editingWire = new WireVisual(
                                sourceNode,
                                (sourceNode != null) ? sourceNode.HighlightedPort.Name : "",
                                (sourceNode != null) ? sourceNode.HighlightedPort.NameHash : 0,
                                targetNode,
                                (targetNode != null) ? targetNode.HighlightedPort.Name : "",
                                (targetNode != null) ? targetNode.HighlightedPort.NameHash : 0,
                                node.HighlightedPort.PortType
                            );
                            m_editingWire.OnMouseMove(mousePos);

                            InvalidateVisual();
                        }
                        
                        // deselect other nodes
                        if (m_selectedNodes.Count > 0)
                        {
                            foreach (BaseVisual visual in m_selectedNodes)
                            {
                                visual.IsSelected = false;
                            }
                            m_selectedNodes.Clear();
                            m_selectedOffsets.Clear();
                        }
                        // highlight node that has port
                        node.IsSelected = true;
                        if (node.Data != null)
                        {
                            SelectedNodeChangedCommand?.Execute(m_hoveredNode.Data);
                        }
                        m_selectedNodes.Add(node);
                        m_selectedOffsets.Add(new Point(node.Rect.Location.X - mousePos.X, node.Rect.Location.Y - mousePos.Y));
                        
                        return;
                    }
                    
                    // select nodes on mouse down for instances where we want to instantly start dragging it
                    foreach (BaseVisual visual in m_visibleNodeVisuals)
                    {
                        if (visual.Rect.Contains(mousePos) && visual.HitTest(mousePos))
                        {
                            // add node to selected nodes if shift is held
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                Mouse.Capture(this);

                                if (!m_selectedNodes.Contains(visual))
                                {
                                    m_selectedNodes.Add(visual);
                                    m_selectedOffsets.Add(new Point(visual.Rect.Location.X - mousePos.X, visual.Rect.Location.Y - mousePos.Y));

                                    visual.IsSelected = true;

                                    if (visual.Data != null)
                                    {
                                        SelectedNodeChangedCommand?.Execute(visual.Data);
                                    }

                                    InvalidateVisual();
                                }
                            }
                            // clear existing selected nodes and select new one
                            else
                            {
                                if (!m_selectedNodes.Contains(visual))
                                {
                                    if (m_selectedNodes.Count > 0)
                                    {
                                        foreach (BaseVisual node in m_selectedNodes)
                                        {
                                            node.IsSelected = false;
                                        }

                                        m_selectedNodes.Clear();
                                        m_selectedOffsets.Clear();
                                    }

                                    m_selectedNodes.Add(visual);
                                    m_selectedOffsets.Add(new Point(visual.Rect.Location.X - mousePos.X, visual.Rect.Location.Y - mousePos.Y));

                                    visual.IsSelected = true;

                                    if (visual.Data != null)
                                    {
                                        SelectedNodeChangedCommand?.Execute(visual.Data);
                                    }
                                    InvalidateVisual();
                                }
                            }

                            for (int i = 0; i < m_selectedOffsets.Count; i++)
                            {
                                m_selectedOffsets[i] = new Point(m_selectedNodes[i].Rect.Location.X - mousePos.X, m_selectedNodes[i].Rect.Location.Y - mousePos.Y);
                            }

                            return;
                        }
                    }
                }
                else
                {
                    Mouse.Capture(this);
                    m_isMarqueeSelecting = m_hoveredNode == null && !Keyboard.IsKeyDown(Key.LeftAlt) && !m_isCuttingWire;
                    m_isCuttingWire = m_hoveredNode == null && Keyboard.IsKeyDown(Key.LeftAlt) && !m_isMarqueeSelecting;
                    m_marqueeSelectionStart = mousePos;
                    m_marqueeSelectionCurrent = mousePos;
                    m_wireCutStart = mousePos;
                    m_wireCutCurrent = mousePos;
                    
                    InvalidateVisual();
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            MatrixTransform m = GetWorldMatrix();
            Point mousePos = m.Inverse.Transform(e.GetPosition(this));

            if (m_editingWire != null)
            {
                if (m_editingWire.HoveredPort != null)
                {
                    BaseNodeVisual.Port sourcePort = (m_editingWire.Source != null) ? m_editingWire.SourcePort : m_editingWire.HoveredPort;
                    BaseNodeVisual.Port targetPort = (m_editingWire.Target != null) ? m_editingWire.TargetPort : m_editingWire.HoveredPort;

                    WireAddedCommand?.Execute(new WireAddedEventArgs(
                        (sourcePort.Owner is NodeVisual) ? (sourcePort.Owner as NodeVisual).Entity : null, 
                        (targetPort.Owner is NodeVisual) ? (targetPort.Owner as NodeVisual).Entity : null, 
                        sourcePort.NameHash, targetPort.NameHash, m_editingWire.WireType
                        ));
                }

                m_editingWire.OnMouseUp(mousePos, e.ChangedButton);
                m_editingWire.Disconnect();
                m_editingWire = null;

                InvalidateVisual();
                return;
            }

            if (m_isMouseMoving && m_isMouseDown)
            {
                m_isMouseMoving = false;
                m_isMouseDown = false;
            }
            else if (!m_isMouseMoving && m_isMouseDown)
            {
                foreach (BaseVisual visual in m_visibleNodeVisuals)
                {
                    if (visual.OnMouseUp(mousePos, e.ChangedButton))
                    {
                        e.Handled = true;
                        InvalidateVisual();
                        return;
                    }
                }

                m_isMouseDown = false;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                Mouse.Capture(null);

                if (UndoManager.Instance.PendingUndoUnit != null)
                {
                    // commit any pending moves
                    UndoManager.Instance.CommitUndo(UndoManager.Instance.PendingUndoUnit);
                }

                foreach (WireVisual wire in m_wireVisuals)
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
                                m_nodeVisuals.Remove(wirePoint);

                                ClearSelection();
                            }));

                        return;
                    }
                }

                if (m_hoveredNode != null)
                {
                    if (PointEqualsWithTolerance(mousePos, prevMousePos))
                    {
                        if (m_selectedNodes.Count > 0 && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        {
                            foreach (BaseVisual node in m_selectedNodes)
                            {
                                node.IsSelected = false;
                            }

                            m_selectedNodes.Clear();
                            m_selectedOffsets.Clear();

                            SelectedNodeChangedCommand?.Execute(null);
                        }

                        if (!m_selectedNodes.Contains(m_hoveredNode))
                        {
                            m_selectedNodes.Add(m_hoveredNode);
                            m_selectedOffsets.Add(new Point(m_hoveredNode.Rect.Location.X - mousePos.X, m_hoveredNode.Rect.Location.Y - mousePos.Y));

                            m_hoveredNode.IsSelected = true;

                            if (m_hoveredNode.Data != null)
                            {
                                SelectedNodeChangedCommand?.Execute(m_hoveredNode.Data);
                            }
                        }
                        InvalidateVisual();
                    }
                }
                else
                {
                    if (!m_isMarqueeSelecting || PointEqualsWithTolerance(mousePos, prevMousePos))
                    {
                        if (m_selectedNodes.Count > 0 && !(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        {
                            foreach (BaseVisual node in m_selectedNodes)
                            {
                                node.IsSelected = false;
                            }

                            m_selectedNodes.Clear();
                            m_selectedOffsets.Clear();

                            SelectedNodeChangedCommand?.Execute(null);
                        }
                    }
                }

                if (m_isMarqueeSelecting)
                {
                    m_isMarqueeSelecting = false;
                    Mouse.Capture(null);

                    InvalidateVisual();
                    return;
                }
                else if (m_isCuttingWire)
                {
                    UndoContainer container = new UndoContainer("Delete Connection(s)");
                
                    m_isCuttingWire = false;
                    foreach (WireVisual wire in m_wiresToCut)
                    {
                        BaseVisual sourceNode = (wire.Source is BaseVisual) ? (wire.Source as BaseVisual) : null;
                        BaseVisual targetNode = (wire.Target is BaseVisual) ? (wire.Target as BaseVisual) : null;
                        
                        WireRemovedCommand?.Execute(new WireRemovedEventArgs(wire.Data,
                                (wire.Source is NodeVisual) ? (wire.Source as NodeVisual).Entity : null,
                                (wire.Target is NodeVisual) ? (wire.Target as NodeVisual).Entity : null, container));
                        
                        sourceNode.Update();
                        targetNode.Update();
                    }
                    
                    if (container.HasItems)
                    {
                        container.Add(new GenericUndoUnit("", o => InvalidateVisual(), o => { }));
                        UndoManager.Instance.CommitUndo(container);
                    }
                    
                    m_wiresToCut.Clear();

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
            {
                return;
            }

            UpdateVisibleNodes();

            if (ItemsSource.World.IsSimulationRunning)
            {
                DrawingContext drawingContext2 = m_debugLayerVisual.RenderOpen();
                RenderDebug(drawingContext2);
                drawingContext2.Close();

                drawingContext2 = m_debugWireVisual.RenderOpen();
                RenderDebugWires(drawingContext2);
                drawingContext2.Close();
            }

            DrawingContextState state = new DrawingContextState(this, drawingContext);
            {
                // draw wires
                for (int i = 0; i < m_wireVisuals.Count; i++)
                {
                    m_wireVisuals[i].Render(state);
                }
                if (m_editingWire != null)
                {
                    m_editingWire.Render(state);
                }

                if (ItemsSource.World.IsSimulationRunning)
                {
                    drawingContext.DrawDrawing(m_debugWireVisual.Drawing);
                }

                // draw nodes
                for (int i = m_visibleNodeVisuals.Count - 1; i >= 0; i--)
                {
                    m_visibleNodeVisuals[i].Render(state);
                }

                if (ItemsSource.World.IsSimulationRunning)
                {
                    drawingContext.DrawDrawing(m_debugLayerVisual.Drawing);
                }

                DrawSelectionRect(drawingContext);
            }
        }
        
        #endregion
        
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
                    m_nodeVisuals.Remove(parentShortcut); 
                    port.ShortcutNode = null; 
                }));
            
            int index = 0;
            int childIndex = 0;

            for (int i = m_wireVisuals.Count - 1; i >= 0; i--)
            {
                WireVisual wire = m_wireVisuals[i];
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
                            m_nodeVisuals.Remove(childToAdd);
                            port.ShortcutChildren.Remove(childToAdd); 
                        }));

                    int indexToDelete = i;
                    WireVisual wireToDelete = m_wireVisuals[indexToDelete];

                    foreach (WirePointVisual wirePoint in wire.WirePoints)
                    {
                        undoContainer.Add(new GenericUndoUnit("",
                            (o) =>
                            {
                                m_nodeVisuals.Remove(wirePoint);
                            }, (o) =>
                            {
                                m_nodeVisuals.Add(wirePoint);
                                wireToDelete.WirePoints.Add(wirePoint);
                            }));
                    }

                    undoContainer.Add(new GenericUndoUnit("",
                        (o) => 
                        {
                            wireToDelete.WirePoints.Clear();
                            m_wireVisuals.Remove(wireToDelete);
                        }, 
                        (o) =>
                        {
                            m_wireVisuals.Insert(indexToDelete, wireToDelete);
                        }));

                    int childIndexToUse = childIndex;
                    WireVisual wireToModify = wire;

                    undoContainer.Add(new GenericUndoUnit("",
                        (o) =>
                        {
                            BaseNodeVisual sourceNode = (port.PortDirection == 0) ? port.ShortcutChildren[childIndexToUse] : wireToModify.Source;
                            targetNode = (port.PortDirection == 0) ? wireToModify.Target : port.ShortcutChildren[childIndexToUse];

                            WireVisual newWire = new WireVisual(sourceNode, wireToModify.SourcePort.Name, wireToModify.SourcePort.NameHash, targetNode, wireToModify.TargetPort.Name, wireToModify.TargetPort.NameHash, wireToModify.WireType);
                            m_wireVisuals.Add(newWire);
                        }, 
                        (o) =>
                        {
                            m_wireVisuals.RemoveAt(m_wireVisuals.Count - 1);
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
                        m_wireVisuals.Add(newWire);
                    }, 
                    (o) =>
                    {
                        m_wireVisuals.RemoveAt(m_wireVisuals.Count - 1);
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
            m_lastFrameCount = ItemsSource.World.SimulationFrameCount;
            InvalidateVisual();
        }
        
        protected void Update()
        {
            m_selectedNodes.Clear();
            m_selectedOffsets.Clear();

            m_hoveredNode = null;

            m_visibleNodeVisuals.Clear();
            m_nodeVisuals.Clear();
            m_wireVisuals.Clear();

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
                    for (int i = m_nodeVisuals.Count - 1; i >= 0; i--)
                    {
                        if (m_nodeVisuals[i] is BaseNodeVisual nodeVisual)
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
                            m_wireVisuals[wireLayout.Index].ApplyLayout(wireLayout, m_nodeVisuals);
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
                                children.Add(m_nodeVisuals.Find(n => n.UniqueId == guid));
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
                        m_nodeVisuals.AddRange(comments);
                    }
                }
                else
                {
                    NodeSortUtils sorter = new NodeSortUtils(m_nodeVisuals, m_wireVisuals);
                    sorter.ApplyAutoLayout();
                }
            }

            InvalidateVisual();
        }

        protected void RenderDebugWires(DrawingContext drawingContext)
        {
            DrawingContextState state = new DrawingContextState(this, drawingContext);

            for (int i = 0; i < m_wireVisuals.Count; i++)
            {
                m_wireVisuals[i].RenderDebug(state);
            }
        }

        protected void RenderDebug(DrawingContext drawingContext)
        {
            DrawingContextState state = new DrawingContextState(this, drawingContext);

            for (int i = m_visibleNodeVisuals.Count - 1; i >= 0; i--)
            {
                m_visibleNodeVisuals[i].RenderDebug(state);
            }

            drawingContext.PushOpacity(0.25);
            drawingContext.DrawText(m_simulationText, new Point(ActualWidth - m_simulationText.Width - 10, ActualHeight - m_simulationText.Height));
            drawingContext.Pop();
        }

        protected override void UpdateScaleParameters()
        {
            base.UpdateScaleParameters();

            m_largeFont.Update(scale);
            m_smallFont.Update(scale);
        }

        private void ClearSelection()
        {
            foreach (BaseVisual node in m_selectedNodes)
            {
                node.IsSelected = false;
            }
            m_selectedNodes.Clear();
            m_selectedOffsets.Clear();

            InvalidateVisual();
        }

        private void AddNode(BaseVisual nodeToAdd)
        {
            foreach (BaseVisual node in m_nodeVisuals.Where(n => n is CommentNodeVisual))
            {
                if (node.Rect.Contains(nodeToAdd.Rect))
                {
                    CommentNodeVisual commentNode = node as CommentNodeVisual;
                    commentNode.AddNode(nodeToAdd);
                }
            }
            m_nodeVisuals.Add(nodeToAdd);
        }

        private void RemoveWire(WireVisual wire)
        {
            wire.Disconnect();
            m_wireVisuals.Remove(wire);
        }

        private void DrawSelectionRect(DrawingContext drawingContext)
        {
            if (!m_isMarqueeSelecting && !m_isCuttingWire)
                return;

            MatrixTransform m = GetWorldMatrix();

            if (m_isCuttingWire)
            {
                drawingContext.DrawLine(m_marqueePen, m.Transform(m_wireCutStart), m.Transform(m_wireCutCurrent));
            }
            else
            {
                double width = m_marqueeSelectionCurrent.X - m_marqueeSelectionStart.X;
                double height = m_marqueeSelectionCurrent.Y - m_marqueeSelectionStart.Y;

                if (width == 0 && height == 0)
                    return;

                Point startPos = m_marqueeSelectionStart;

                if (width <= 0)
                {
                    width = m_marqueeSelectionStart.X - m_marqueeSelectionCurrent.X;
                    startPos.X = m_marqueeSelectionCurrent.X;
                }
                if (height <= 0)
                {
                    height = m_marqueeSelectionStart.Y - m_marqueeSelectionCurrent.Y;
                    startPos.Y = m_marqueeSelectionCurrent.Y;
                }

                drawingContext.DrawRectangle(null, m_marqueePen, new Rect(m.Transform(startPos), new Size(width * scale, height * scale)));
            }
        }

        private void UpdateVisibleNodes()
        {
            MatrixTransform m = GetWorldMatrix();
            Point topLeft = m.Inverse.Transform(new Point(0, 0));
            Point bottomRight = m.Inverse.Transform(new Point(ActualWidth, ActualHeight));
            Rect frustum = new Rect(topLeft, bottomRight);

            m_visibleNodeVisuals.Clear();
            foreach (BaseVisual visual in m_nodeVisuals)
            {
                if (frustum.IntersectsWith(visual.Rect) || visual.IsSelected)
                {
                    if (visual.IsSelected)
                    {
                        m_visibleNodeVisuals.Insert(0, visual);
                    }
                    else
                    {
                        m_visibleNodeVisuals.Add(visual);
                    }
                }
            }
        }

        private void GenerateNodes(IEnumerable<Entity> entities)
        {
            //
            // Port Context Menu
            //
            ContextMenu portContextMenu = new ContextMenu();
            
            // shortcut
            MenuItem makeShortcutMenuItem = new MenuItem() { Header = "Make Shortcut" };
            makeShortcutMenuItem.Click += (o, e) =>
            {
                BaseNodeVisual.Port port = (o as MenuItem).DataContext as BaseNodeVisual.Port;
                GenerateShortcuts(port.Owner, port);
            };
            portContextMenu.Items.Add(makeShortcutMenuItem);

            // copy name
            MenuItem copyNameMenuItem = new MenuItem() { Header = "Copy Name" };
            copyNameMenuItem.Click += (o, e) =>
            {
                BaseNodeVisual.Port port = (o as MenuItem).DataContext as BaseNodeVisual.Port;
                Clipboard.SetText(port.Name);
                
                Frosty.Core.App.NotificationManager.Show($"Copied {port.Name}");
            };
            portContextMenu.Items.Add(copyNameMenuItem);
            
            //
            // Node Generation
            //
            
            Random r = new Random(2);
            foreach (Entity entity in entities)
            {
                ILogicEntity logicEntity = entity as ILogicEntity;

                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;
                bool isCollapsed = entity.UserData == null;

                x = x - (x % 10);
                y = y - (y % 10);
                ContextMenu nodeContextMenu = new ContextMenu();
                NodeVisual node = new NodeVisual(logicEntity, x, y) 
                { 
                    GlyphWidth = OriginalAdvanceWidth, 
                    NodeContextMenu = nodeContextMenu,
                    PortContextMenu = portContextMenu, 
                    UniqueId = Guid.NewGuid() 
                };

                //
                // Logic Entity Context Menu
                //
                if (logicEntity is LogicEntity l)
                {
                    // add dynamic port
                    MenuItem addPortMenuItem = new MenuItem() { Header = "Add Dynamic Port" };
                    addPortMenuItem.Click += (o, e) =>
                    {
                        SchematicsDynamicPortWindow window = new SchematicsDynamicPortWindow();
                        if (window.ShowDialog() == true)
                        {
                            if (window.Options is DynamicPortSettings portSettings)
                            {
                                BaseNodeVisual.Port port = new BaseNodeVisual.Port(node)
                                {
                                    IsDynamicallyGenerated = true,
                                    Name = portSettings.Name,
                                    NameHash = SchematicsUtils.HashString(portSettings.Name),
                                    PortType = (int)portSettings.Type,
                                    PortDirection = 1 - (int)portSettings.Direction
                                };
                                
                                bool isInPort = portSettings.Direction == Direction.In;
                                // property
                                if (portSettings.Type == PortType.Property)
                                {
                                    if (isInPort)
                                    {
                                        node.InputProperties.Add(port);
                                    }
                                    else
                                    {
                                        node.OutputProperties.Add(port);
                                    }
                                }
                                // event
                                else if (portSettings.Type == PortType.Event)
                                {
                                    if (isInPort)
                                    {
                                        node.InputEvents.Add(port);
                                    }
                                    else
                                    {
                                        node.OutputEvents.Add(port);
                                    }
                                }
                                // link
                                else if (portSettings.Type == PortType.Link)
                                {
                                    if (isInPort)
                                    {
                                        node.InputLinks.Add(port);
                                    }
                                    else
                                    {
                                        node.OutputLinks.Add(port);
                                    }
                                }
                                
                                node.Update();
                                InvalidateVisual();
                            }
                        }
                    };
                    
                    nodeContextMenu.Items.Add(addPortMenuItem);
                    
                    // trigger events
                    MenuItem triggerMenuItem = new MenuItem() { Header = "Trigger Event" };

                    MenuItem inputTriggerMenuItem = new MenuItem() { Header = "Input" };
                    MenuItem outputTriggerMenuItem = new MenuItem() { Header = "Output" };
                    
                    foreach (ConnectionDesc conn in l.Events)
                    {
                        MenuItem eventMenuItem = new MenuItem() { Header = conn.Name };
                        eventMenuItem.Click += (o, e) => { l.EventToTrigger = Frosty.Hash.Fnv1.HashString(conn.Name); };
                        
                        if (conn.Direction == Direction.In)
                        {
                            inputTriggerMenuItem.Items.Add(eventMenuItem);
                        }
                        else if (conn.Direction == Direction.Out)
                        {
                            outputTriggerMenuItem.Items.Add(eventMenuItem);
                        }
                    }

                    bool hasTriggers = false;
                    if (inputTriggerMenuItem.Items.Count > 0)
                    {
                        triggerMenuItem.Items.Add(inputTriggerMenuItem);
                        hasTriggers = true;
                    }
                    if (outputTriggerMenuItem.Items.Count > 0)
                    {
                        triggerMenuItem.Items.Add(outputTriggerMenuItem);
                        hasTriggers = true;
                    }

                    if (hasTriggers)
                    {
                        nodeContextMenu.Items.Add(triggerMenuItem);
                    }
                }
                
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
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_Source: m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() }); break;
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_Target: m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() }); break;
                    case FrostySdk.Ebx.FieldAccessType.FieldAccessType_SourceAndTarget:
                        m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth });
                        m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, dataField, x, y) { GlyphWidth = OriginalAdvanceWidth });
                        break;
                }
            }

            foreach (DynamicEvent eventField in interfaceDesc.Data.InputEvents)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, eventField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
            foreach (DynamicEvent eventField in interfaceDesc.Data.OutputEvents)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, eventField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }

            foreach (DynamicLink linkField in interfaceDesc.Data.InputLinks)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 0, linkField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
            foreach (DynamicLink linkField in interfaceDesc.Data.OutputLinks)
            {
                double x = ((int)(r.NextDouble() * 2000)) - 1000;
                double y = ((int)(r.NextDouble() * 2000)) - 1000;

                x = x - (x % 10);
                y = y - (y % 10);

                m_nodeVisuals.Add(new InterfaceNodeVisual(ItemsSource.BlueprintGuid, interfaceDesc, 1, linkField, x, y) { GlyphWidth = OriginalAdvanceWidth, UniqueId = Guid.NewGuid() });
            }
        }

        private void GenerateWires(IEnumerable<object> connections, int connectionType)
        {
            IEnumerable<BaseNodeVisual> nodes = m_nodeVisuals.Where(n => n is BaseNodeVisual).Select(n => n as BaseNodeVisual);
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

                m_wireVisuals.Add(wire);
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
                    BaseVisual visual = m_nodeVisuals.Find(n => n.Data == entity);
                    
                    m_nodeVisuals.Remove(visual);
                    
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
                    WireVisual wire = m_wireVisuals.Find(n => n.Data == connection);
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
                    WireVisual wire = m_wireVisuals.Find(n => n.Data == connection);
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
                    WireVisual wire = m_wireVisuals.Find(n => n.Data == connection);
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
            foreach (BaseVisual node in canvas.m_visibleNodeVisuals)
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
