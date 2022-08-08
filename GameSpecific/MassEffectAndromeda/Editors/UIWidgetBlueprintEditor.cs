using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using LevelEditorPlugin.Controls;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Layers;
using LevelEditorPlugin.Library.Reflection;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Frosty.Core.Managers;

namespace LevelEditorPlugin.Editors
{
    public class EventHashToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return null;

            return FrostySdk.Utils.GetString((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [TemplatePart(Name = "PART_Canvas", Type = typeof(UIWidgetCanvas))]
    public class UIWidgetBlueprintEditor : ToolbarAssetEditor, IEditorProvider, ITimelineUpdateProvider, ISchematicsInterfaceProvider, ISimulationUpdateProvider, IDragDropTargetProvider, IWorldProvider
    {
        public static readonly DependencyProperty IsGridVisibleProperty = DependencyProperty.Register("IsGridVisible", typeof(bool), typeof(UIWidgetBlueprintEditor), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty IsDebugOutlinesVisibleProperty = DependencyProperty.Register("IsDebugOutlinesVisible", typeof(bool), typeof(UIWidgetBlueprintEditor), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty IsInGameViewProperty = DependencyProperty.Register("IsInGameView", typeof(bool), typeof(UIWidgetBlueprintEditor), new FrameworkPropertyMetadata(false));

        public bool IsGridVisible
        {
            get => (bool)GetValue(IsGridVisibleProperty);
            set => SetValue(IsGridVisibleProperty, value);
        }
        public bool IsDebugOutlinesVisible
        {
            get => (bool)GetValue(IsDebugOutlinesVisibleProperty);
            set => SetValue(IsDebugOutlinesVisibleProperty, value);
        }
        public bool IsInGameView
        {
            get => (bool)GetValue(IsInGameViewProperty);
            set => SetValue(IsInGameViewProperty, value);
        }

        public LevelEditorScreen Screen => null;
        public SceneLayer RootLayer => rootLayer;
        public DockManager DockManager => dockManager;
        public UIWidgetEntity WidgetEntity => entity.RootEntity;
        public InterfaceDescriptor Interface => entity.InterfaceDescriptor;
        public EntityWorld World => world;

#pragma warning disable CS0067
        public event EventHandler<SelectedEntityChangedEventArgs> SelectedEntityChanged;
        public event EventHandler<SelectedLayerChangedEventArgs> SelectedLayerChanged;
#pragma warning restore CS0067

        protected DockManager dockManager;
        protected UIElementWidgetReferenceEntity entity;
        protected SceneLayer rootLayer;
        protected Entity selectedEntity;
        protected EntityWorld world;

        protected IEnumerable<int> schematicEvents;
        protected int selectedEvent;
        
        private bool simulationInput;
        private UIWidgetCanvas canvas;

        public event EventHandler<InterfaceOutputPropertyChangedEventArgs> OnInterfaceOutputPropertyChanged;
        public event EventHandler<InterfaceOutputEventTriggeredEventArgs> OnInterfaceOutputEventTriggered;
        public event EventHandler OnSimulationUpdated;

        static UIWidgetBlueprintEditor()
        {
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/LevelEditorPlugin;component/GameSpecific/MassEffectAndromeda/Themes/Editors/UIWidgetBlueprintEditor.xaml", UriKind.Relative)) as ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/LevelEditorPlugin;component/GameSpecific/MassEffectAndromeda/Themes/Editors/ViewModels/UIWidgetLayersInstancesViewModel.xaml", UriKind.Relative)) as ResourceDictionary);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UIWidgetBlueprintEditor), new FrameworkPropertyMetadata(typeof(UIWidgetBlueprintEditor)));
        }

        public UIWidgetBlueprintEditor(ILogger inLogger)
            : base(inLogger)
        {
            dockManager = new DockManager(this);
            dockManager.LoadFromConfig("UIWidgetBlueprintEditor", new DockManager.DockManagerConfigData()
            {
                Layouts = new List<DockManager.DockLayoutData>()
                {
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Properties",
                        IsVisible = true,
                        IsSelected = true,
                        Location = DockLocation.TopRight
                    },
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Interface",
                        IsVisible = false,
                        IsSelected = false,
                        Location = DockLocation.TopLeft,
                    },
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Timeline",
                        IsVisible = false,
                        IsSelected = false,
                        Location = DockLocation.Bottom,
                    },
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Schematics",
                        IsVisible = false,
                        IsSelected = false,
                        Location = DockLocation.Floating,
                        FloatingData = new DockManager.DockLayoutFloatingData()
                        {
                             Width = 800,
                             Height = 400
                        }
                    },
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_WidgetToolbox",
                        IsVisible = false,
                        IsSelected = false,
                        Location = DockLocation.BottomLeft,
                    },
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_WidgetLayers",
                        IsVisible = true,
                        IsSelected = true,
                        Location = DockLocation.TopLeft,
                    },
                }
            });
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new DockingToolbarItem("", "Show/Hide properties tab", "Images/Properties.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new PropertiesViewModel(this, selectedEntity, new RelayCommand(PropertyGridModified)))), DockManager, "UID_LevelEditor_Properties"),
                new DockingToolbarItem("", "Show/Hide interface tab", "Images/Interface.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new InterfaceViewModel(this))), DockManager, "UID_LevelEditor_Interface"),
                new DockingToolbarItem("", "Show/Hide toolbox", "Images/Toolbox.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new UIWidgetToolboxViewModel())), DockManager, "UID_LevelEditor_WidgetToolbox"),
                new DockingToolbarItem("", "Show/Hide layers/instances", "Images/Layers.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new UIWidgetLayersInstancesViewModel(this))), DockManager, "UID_LevelEditor_WidgetLayers"),
                new DockingToolbarItem("", "Show/Hide timeline editor", "Images/Timeline.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new TimelineViewModel(this))), DockManager, "UID_LevelEditor_Timeline"),
                new FloatingOnlyDockingToolbarItem("", "Show/Hide schematics editor", "Images/Schematics.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new SchematicsViewModel(this, rootLayer))), DockManager, "UID_LevelEditor_Schematics"),
                new DividerToolbarItem(),
                new ToggleToolbarItem("", "Show/Hide the grid", "FrostyEditor/Images/Grid.png", true, new RelayCommand((o) => { IsGridVisible = !IsGridVisible; (o as ToggleToolbarItem).IsToggled = IsGridVisible; })),
                new ToggleToolbarItem("", "Show/Hide the widget debug outlines", "LevelEditorPlugin/Images/ThumbnailSafezone.png", true, new RelayCommand((o) => { IsDebugOutlinesVisible = !IsDebugOutlinesVisible; (o as ToggleToolbarItem).IsToggled = IsDebugOutlinesVisible; })),
                new ToggleToolbarItem("", "Enter/Exit simulation", "LevelEditorPlugin/Images/GameView.png", false, new RelayCommand((o) => { IsInGameView = !IsInGameView; (o as ToggleToolbarItem).IsToggled = IsInGameView; OnIsInGameViewChanged(); })),
                new ToggleToolbarItem("", "Enable/Disable input", "LevelEditorPlugin/Images/Input.png", false, new RelayCommand((o) => { simulationInput = !simulationInput; (o as ToggleToolbarItem).IsToggled = simulationInput; })),
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PerformTemplateMagic();

            canvas = GetTemplateChild("PART_Canvas") as UIWidgetCanvas;
        }

        public void CenterOnSelection()
        {
            throw new NotImplementedException();
        }

        public void SelectEntity(Entity newSelection)
        {
            if (newSelection != selectedEntity)
            {
                // Select the root world if nothing else is selected
                var tmpSelection = newSelection;
                if (tmpSelection == null)
                    tmpSelection = entity;

                SelectedEntityChanged?.Invoke(this, new SelectedEntityChangedEventArgs(tmpSelection, selectedEntity));
                selectedEntity = newSelection;
            }
        }

        public void SelectLayer(SceneLayer newSelection)
        {
        }

        public override void Closed()
        {
            dockManager.SaveToConfig("UIWidgetBlueprintEditor");
            dockManager.Shutdown();

            world.Shutdown();
            entity.Destroy();

            base.Closed();
        }

        public void TimelinePlaybackUpdate(float currentTime)
        {
            Dispatcher.Invoke(() =>
            {
                canvas.Update();
            });
        }

        public void InterfacePropertyPushed(int propertyHash, string newValue)
        {
            if (world.IsSimulationRunning)
            {
                entity.InterfaceDescriptor?.QueueProperty(propertyHash, newValue);
            }
        }

        public void InterfaceEventPushed(int eventHash)
        {
            if (world.IsSimulationRunning)
            {
                entity.InterfaceDescriptor?.QueueEvent(eventHash);
            }
        }

        public bool IsValidDropSource(IDataObject draggedData)
        {
            if (draggedData.GetDataPresent(typeof(UIWidgetDropData)))
                return true;

            if (draggedData.GetDataPresent(typeof(DataExplorerDropData)))
            {
                var dropData = (DataExplorerDropData)draggedData.GetData(typeof(DataExplorerDropData));
                if (dropData.Entry.Type == "UIWidgetBlueprint")
                    return true;
            }

            return false;
        }

        public void ProcessDrop(IDataObject draggedData, System.Windows.Point dropPoint)
        {
            if (draggedData.GetDataPresent(typeof(UIWidgetDropData)))
            {
                var dropData = (UIWidgetDropData)draggedData.GetData(typeof(UIWidgetDropData));
                var attr = dropData.DataType.GetCustomAttribute<EntityBindingAttribute>();

                var refObjEntity = rootLayer.Entity as UIElementWidgetReferenceEntity;
                var layerEntity = refObjEntity.RootEntity.Layers.First();

                var entity = (UIElementEntity)Activator.CreateInstance(dropData.DataType, new object[] { CreateEntityData(attr.DataType), layerEntity });

                if (entity != null)
                {
                    entity.SetDefaultValues();
                    entity.UserData = dropPoint;

                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add UIWidget",
                        (o) =>
                        {
                            // add entity to ebx
                            Asset.AddObject(entity.GetRawData());

                            // add to reference
                            layerEntity.AddEntity(entity);
                            entity.PerformLayout();
                            LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);

                            // add to canvas
                            canvas.AddEntity(entity);

                            // select the new entity
                            SelectEntity(entity);

                        },
                        (o) =>
                        {
                            // remove from reference
                            layerEntity.RemoveEntity(entity);

                            // remove from ebx
                            Asset.RemoveObject(entity.GetRawData());
                            LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);

                            // remove from canvas
                            canvas.RemoveEntity(entity);

                        }));
                }
            }
            else if (draggedData.GetDataPresent(typeof(DataExplorerDropData)))
            {
                var dropData = (DataExplorerDropData)draggedData.GetData(typeof(DataExplorerDropData));
                var asset = App.AssetManager.GetEbx(dropData.Entry);

                var refObjEntity = rootLayer.Entity as UIElementWidgetReferenceEntity;
                var layerEntity = refObjEntity.RootEntity.Layers.First();

                var entityData = (FrostySdk.Ebx.UIElementWidgetReferenceEntityData)CreateEntityData(typeof(FrostySdk.Ebx.UIElementWidgetReferenceEntityData));
                entityData.Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = dropData.Entry.Guid, ClassGuid = asset.RootInstanceGuid });

                var entity = (UIElementWidgetReferenceEntity)Activator.CreateInstance(typeof(UIElementWidgetReferenceEntity), new object[] { entityData, layerEntity });

                if (entity != null)
                {
                    entity.SetDefaultValues();
                    entity.UserData = dropPoint;

                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add UIWidget",
                        (o) =>
                        {
                            // add entity to ebx
                            Asset.AddObject(entity.GetRawData());

                            // add to reference
                            layerEntity.AddEntity(entity);
                            entity.PerformLayout();
                            LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);

                            // add to canvas
                            canvas.AddEntity(entity);

                            // select the new entity
                            SelectEntity(entity);

                        },
                        (o) =>
                        {
                            // remove from reference
                            layerEntity.RemoveEntity(entity);

                            // remove from ebx
                            Asset.RemoveObject(entity.GetRawData());
                            LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);

                            // remove from canvas
                            canvas.RemoveEntity(entity);

                        }));
                }
            }
        }

        protected override void Reload()
        {
            dockManager.ShowFloatingWindows();
        }

        protected override void Unload()
        {
            dockManager.HideFloatingWindows();
        }

        protected override void Initialize()
        {
            FrostySdk.Ebx.UIElementWidgetReferenceEntityData objectData = new FrostySdk.Ebx.UIElementWidgetReferenceEntityData()
            {
                Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = Asset.FileGuid, ClassGuid = Asset.RootInstanceGuid }),
                Alpha = 1.0f
            };

            world = new EntityWorld();
            world.OnSimulationUpdated += SimulationUpdated;

            entity = Entity.CreateEntity(objectData, Asset.FileGuid, world) as UIElementWidgetReferenceEntity;
            entity.PerformLayout();

            rootLayer = MakeFakeLayer();

            dockManager.AddItemOnLoad(new PropertiesViewModel(this, null, new RelayCommand(PropertyGridModified)));
            dockManager.AddItemOnLoad(new InterfaceViewModel(this));
            dockManager.AddItemOnLoad(new UIWidgetToolboxViewModel());
            dockManager.AddItemOnLoad(new UIWidgetLayersInstancesViewModel(this));
            dockManager.AddItemOnLoad(new TimelineViewModel(this));
            dockManager.AddItemOnLoad(new SchematicsViewModel(this, rootLayer));

            canvas.Widget = entity.RootEntity;
            if (entity.InterfaceDescriptor != null)
            {
                entity.InterfaceDescriptor.OnInterfaceOutputPropertyChanged += InterfaceOutputPropertyChanged;
                entity.InterfaceDescriptor.OnInterfaceOutputEventTriggered += InterfaceOutputEventTriggered;
            }

            canvas.SelectedEntityChanged += Canvas_SelectedEntityChanged;

            base.Initialize();
        }

        // @temp
        protected SceneLayer MakeFakeLayer()
        {
            string layerName = Path.GetFileName(entity.Blueprint.Name);
            SceneLayer layer = new SceneLayer(entity, layerName, new SharpDX.Color(0.0f, 0.5f, 0.0f, 1.0f));

            List<Entity> entities = (List<Entity>)entity.GetType().GetField("entities", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(entity);
            foreach (var entity in entities)
            {
                RecursiveAddToLayer(entity, layer);
            }

            return layer;
        }

        protected void RecursiveAddToLayer(Entity entity, SceneLayer layer)
        {
            layer.AddEntity(entity);
            entity.SetOwner(entity);

            if (entity is UIWidgetEntity)
            {
                var widgetEntity = entity as UIWidgetEntity;
                foreach (var widgetLayer in widgetEntity.Layers)
                {
                    RecursiveAddToLayer(widgetLayer, layer);
                }
            }
            else if (entity is UIElementLayerEntity)
            {
                var widgetLayer = entity as UIElementLayerEntity;
                foreach (var element in widgetLayer.Elements)
                {
                    RecursiveAddToLayer(element, layer);
                }
            }
            else if (entity is UIContainerEntity)
            {
                var containerEntity = entity as UIContainerEntity;
                foreach (var element in containerEntity.Elements)
                {
                    RecursiveAddToLayer(element, layer);
                }
            }
        }

        protected void OnIsInGameViewChanged()
        {
            if (IsInGameView)
            {
                world.BeginSimulation();
            }
            else
            {
                world.EndSimulation();
            }

            App.NotificationManager.Show($"Simulation {((IsInGameView) ? "Started" : "Stopped")}");
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (simulationInput)
            {
                e.Handled = true;
                System.Windows.Point mousePos = canvas.TransformPoint(e.GetPosition(canvas));
                entity.OnMouseMove(mousePos);
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (simulationInput)
            {
                e.Handled = true;
                System.Windows.Point mousePos = canvas.TransformPoint(e.GetPosition(canvas));
                entity.OnMouseDown(mousePos, e.ChangedButton);
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (simulationInput)
            {
                e.Handled = true;
                System.Windows.Point mousePos = canvas.TransformPoint(e.GetPosition(canvas));
                entity.OnMouseUp(mousePos, e.ChangedButton);
            }

            base.OnPreviewMouseUp(e);
        }

        private void SimulationUpdated(object sender, EventArgs e)
        {
            canvas?.Update();
            OnSimulationUpdated?.Invoke(sender, e);
        }

        private void InterfaceOutputPropertyChanged(object sender, InterfaceOutputPropertyChangedEventArgs e)
        {
            OnInterfaceOutputPropertyChanged?.Invoke(sender, e);
        }

        private void InterfaceOutputEventTriggered(object sender, InterfaceOutputEventTriggeredEventArgs e)
        {
            OnInterfaceOutputEventTriggered?.Invoke(sender, e);
        }

        private object CreateEntityData(Type entityDataType)
        {
            FrostySdk.Ebx.DataBusPeer entityData = (FrostySdk.Ebx.DataBusPeer)Activator.CreateInstance(entityDataType);

            Guid instanceGuid = FrostySdk.Utils.GenerateDeterministicGuid(Asset.Objects, entityDataType.Name, Asset.FileGuid);
            entityData.SetInstanceGuid(new FrostySdk.Ebx.AssetClassGuid(instanceGuid, -1));

            byte[] array = instanceGuid.ToByteArray();
            uint flagsValue = (uint)((int)(array[3] & 0x01) << 24 | (int)array[2] << 16 | (int)array[1] << 8 | (int)array[0]);

            entityData.Flags = flagsValue;
            return entityData;
        }

        private void Canvas_SelectedEntityChanged(object sender, SelectedEntityChangedEventArgs e)
        {
            SelectEntity(e.NewSelection);
        }

        private void PropertyGridModified(object obj)
        {
            selectedEntity.OnDataModified();
            (selectedEntity as IUIWidget).PerformLayout();
            canvas.Update();
        }
    }
}
