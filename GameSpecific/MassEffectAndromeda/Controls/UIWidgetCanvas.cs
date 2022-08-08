using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls
{
    public class UIWidgetCanvas : GridCanvas
    {
        public static readonly DependencyProperty WidgetProperty = DependencyProperty.Register("Widget", typeof(object), typeof(UIWidgetCanvas), new FrameworkPropertyMetadata(null, OnWidgetChanged));
        public static readonly DependencyProperty DebugOutlinesVisibleProperty = DependencyProperty.Register("DebugOutlinesVisible", typeof(bool), typeof(UIWidgetCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public object Widget
        {
            get => GetValue(WidgetProperty);
            set => SetValue(WidgetProperty, value);
        }
        public bool DebugOutlinesVisible
        {
            get => (bool)GetValue(DebugOutlinesVisibleProperty);
            set => SetValue(DebugOutlinesVisibleProperty, value);
        }

        protected Size widgetSize;
        protected List<WidgetProxy> proxies = new List<WidgetProxy>();
        protected List<WidgetProxy> simulationProxies = new List<WidgetProxy>();

        private Pen widgetBorderPen;
        private Entities.Entity selectedEntity;

        public event EventHandler<SelectedEntityChangedEventArgs> SelectedEntityChanged;

        public UIWidgetCanvas()
        {
            widgetBorderPen = new Pen(Brushes.White, 1);
            widgetBorderPen.Freeze();
        }

        public void Update()
        {
            foreach (var proxy in proxies.Union(simulationProxies))
            {
                proxy.Update();
            }

            InvalidateVisual();
        }

        public void AddEntity(Entities.Entity entity)
        {
            //SpawnProxies(new[] { entity });
            InvalidateVisual();
        }

        public void RemoveEntity(Entities.Entity entity)
        {
            RemoveProxies(new[] { entity }, false);
            InvalidateVisual();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            Point currentPos = TransformPoint(e.GetPosition(this));
            foreach (var proxy in proxies)
            {
                if (proxy.Bounds.Contains(currentPos))
                {
                    if (selectedEntity != proxy.Entity)
                    {
                        SelectedEntityChanged?.Invoke(this, new SelectedEntityChangedEventArgs(proxy.Entity.Owner, (selectedEntity != null) ? selectedEntity.Owner : null));
                        selectedEntity = proxy.Entity.Owner;
                        break;
                    }
                }
            }
        }

        protected void Reset()
        {
            proxies.Clear();
            if (Widget != null)
            {
                var widgetEntity = Widget as Entities.UIWidgetEntity;
                widgetSize = new Size(widgetEntity.Data.Size.X, widgetEntity.Data.Size.Y);

                //SpawnProxies(widgetEntity.World.Entities);

                gridOffset = new Point(widgetSize.Width * 0.5, widgetSize.Height * 0.5);
                offset = new TranslateTransform(widgetSize.Width * 0.5, widgetSize.Height * 0.5);
            }
        }

        protected void SpawnProxies(IEnumerable<Entities.Entity> entities)
        {
            foreach (var element in entities)
            {
                if (element is Entities.IUIWidget)
                {
                    if (!element.HasFlags(Entities.EntityFlags.RenderProxyGenerated))
                    {
                        var widget = element as Entities.IUIWidget;
                        var proxy = widget.CreateRenderProxy();

                        if (proxy != null)
                        {
                            proxies.Add(proxy);
                        }
                    }
                }
            }
        }

        protected void RemoveProxies(IEnumerable<Entities.Entity> entities, bool simulationRunning)
        {
            foreach (var element in entities)
            {
                if (element is Entities.IUIWidget)
                {
                    var widget = element as Entities.IUIWidget;
                    simulationProxies.RemoveAll(p => p.Entity == element);

                    if (element is Entities.UIContainerEntity)
                    {
                        var containerEntity = element as Entities.UIContainerEntity;
                        RemoveProxies(containerEntity.Elements, simulationRunning);
                    }
                    else if (element is Entities.UIElementWidgetReferenceEntity)
                    {
                        var widgetRef = element as Entities.UIElementWidgetReferenceEntity;
                        foreach (var layer in widgetRef.RootEntity.Layers)
                        {
                            RemoveProxies(layer.Elements, simulationRunning);
                        }
                    }
                }
            }
        }

        protected override void Render(DrawingContext drawingContext)
        {
            var widgetEntity = Widget as Entities.UIWidgetEntity;
            if (widgetEntity != null)
            {
                SpawnProxies(widgetEntity.World.Entities);
            }

            //if (SchematicsSimulationWorld.EntitiesToRemove.Count > 0)
            //{
            //    RemoveProxies(SchematicsSimulationWorld.EntitiesToRemove, true);
            //    foreach (var entity in SchematicsSimulationWorld.EntitiesToRemove)
            //    {
            //        entity.Destroy();
            //    }
            //    SchematicsSimulationWorld.EntitiesToRemove.Clear();
            //}

            drawingContext.PushTransform(GetWorldMatrix());

            foreach (var proxy in proxies.Union(simulationProxies))
            {
                proxy.DrawDebugOutlines = DebugOutlinesVisible;
                if (proxy.IsVisible)
                {
                    proxy.Render(drawingContext);
                }
            }

            if (DebugOutlinesVisible)
            {
                drawingContext.DrawRectangle(null, widgetBorderPen, new Rect(0, 0, widgetSize.Width, widgetSize.Height));
            }

            drawingContext.Pop();
        }

        private static void OnWidgetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var widgetCanvas = o as UIWidgetCanvas;
            widgetCanvas.Reset();
        }
    }
}
