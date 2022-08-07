using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.IO;
using LevelEditorPlugin.Controls;
using LevelEditorPlugin.Layers;
using LevelEditorPlugin.Library.Reflection;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Frosty.Core.Managers;

namespace LevelEditorPlugin.Editors
{
    //public static class SchematicsSimulationWorld
    //{
    //    public static bool IsSimulationRunning { get; set; }
    //    public static List<Entities.Entity> EntitiesToCreate { get; set; } = new List<Entities.Entity>();
    //    public static List<Entities.Entity> EntitiesToRemove { get; set; } = new List<Entities.Entity>();
    //    public static long FrameCount { get; set; } = -1;
    //    public static System.Windows.Threading.Dispatcher UiDispatcher { get; set; } = Application.Current.Dispatcher;
    //    public static System.Windows.Threading.Dispatcher SimDispatcher { get; set; }
    //}

    public class EntityWorld
    {
        private class SimulatedEntityWorld
        {
            public IEnumerable<Entities.Entity> Entities => simulatedEntities;
            public Entities.ILogicEntity RootEntity => originalWorld.Entities.First() as Entities.ILogicEntity;
            public long FrameCount => frameCount;
            public Dispatcher UiDispatcher => uiDispatcher;
            public Dispatcher SimulationDispatcher => simulationDispatcher;

            protected EntityWorld originalWorld;
            protected List<Entities.Entity> simulatedEntities = new List<Entities.Entity>();
            protected List<Entities.ReferenceObject> simulatedReferenceObjects = new List<Entities.ReferenceObject>();
            protected long frameCount;

            protected Thread simulationThread;
            protected Dispatcher simulationDispatcher;
            protected Dispatcher uiDispatcher;
            protected bool simulationRunning;
            protected Stopwatch timer;

            public SimulatedEntityWorld(EntityWorld inOriginalWorld)
            {
                originalWorld = inOriginalWorld;
                uiDispatcher = Application.Current.Dispatcher;

                simulationThread = new Thread(SimulationThread);
                simulationThread.Start();
            }

            public void AddEntity(Entities.Entity inEntity)
            {
                simulatedEntities.Add(inEntity);
                if (inEntity is Entities.ReferenceObject)
                {
                    simulatedReferenceObjects.Add(inEntity as Entities.ReferenceObject);
                }
            }

            public void RemoveEntity(Entities.Entity inEntity)
            {
                simulatedEntities.Remove(inEntity);
                if (inEntity is Entities.ReferenceObject)
                {
                    simulatedReferenceObjects.Remove(inEntity as Entities.ReferenceObject);
                }
            }

            public void Shutdown()
            {
                simulationRunning = false;
            }

            private void SimulationThread(object state)
            {
                var dispatcherReady = new TaskCompletionSource<Dispatcher>();
                var dispatcherThread = new Thread(() =>
                {
                    dispatcherReady.SetResult(Dispatcher.CurrentDispatcher);
                    Dispatcher.Run();
                });
                dispatcherThread.IsBackground = true;
                dispatcherThread.Name = "Simulation Dispatcher";
                dispatcherThread.Start();

                frameCount = -1;
                simulationDispatcher = dispatcherReady.Task.Result;
                simulationRunning = true;

                try
                {
                    timer = new Stopwatch();
                    timer.Start();

                    RootEntity.BeginSimulation();

                    while (simulationRunning)
                    {
                        RootEntity.Update_PreFrame();
                        if (timer.ElapsedMilliseconds > 60)
                        {
                            uiDispatcher.Invoke(() => { originalWorld.OnSimulationUpdated?.Invoke(this, new EventArgs()); });
                            frameCount++;
                            timer.Restart();
                        }
                        RootEntity.Update_PostFrame();
                    }

                    RootEntity.EndSimulation();
                    timer.Stop();
                }
                catch (Exception)
                {
                    simulationDispatcher.InvokeShutdown();
                    dispatcherThread.Join();
                    return;
                }

                // to remove the debug layer
                uiDispatcher.Invoke(() => { originalWorld.OnSimulationUpdated?.Invoke(this, new EventArgs()); });

                simulationDispatcher.InvokeShutdown();
                dispatcherThread.Join();
            }
        }

        public bool IsSimulationRunning => simulation != null;
        public IEnumerable<Entities.Entity> Entities
        {
            get
            {
                IEnumerable<Entities.Entity> allEntities = entities;
                if (IsSimulationRunning)
                {
                    return allEntities.Union(simulation.Entities);
                }
                return allEntities;
            }
        }
        public long SimulationFrameCount => (IsSimulationRunning) ? simulation.FrameCount : -1;
        public Dispatcher UiDispatcher => (IsSimulationRunning) ? simulation.UiDispatcher : null;
        public Dispatcher SimulationDispatcher => (IsSimulationRunning) ? simulation.SimulationDispatcher : null;

        protected List<Entities.Entity> entities = new List<Entities.Entity>();
        protected List<Entities.ReferenceObject> referenceObjects = new List<Entities.ReferenceObject>();

        private SimulatedEntityWorld simulation;
        public event EventHandler OnSimulationUpdated;

        public EntityWorld()
        {
        }

        public void Initialize()
        {
            foreach (var entity in Entities)
            {
                if (entity is Entities.ReferenceObject)
                {
                    (entity as Entities.ReferenceObject).InitializeSchematics();
                }
            }
        }

        public void BeginSimulation()
        {
            simulation = new SimulatedEntityWorld(this);
        }

        public void EndSimulation()
        {
            if (IsSimulationRunning)
            {
                simulation.Shutdown();
                simulation = null;
            }
        }

        public void Shutdown()
        {
            EndSimulation();
        }

        public void AddEntity(Entities.Entity inEntity)
        {
            if (IsSimulationRunning)
            {
                simulation.AddEntity(inEntity);
            }
            else
            {
                entities.Add(inEntity);
                if (inEntity is Entities.ReferenceObject)
                {
                    referenceObjects.Add(inEntity as Entities.ReferenceObject);
                }
            }
        }

        public void RemoveEntity(Entities.Entity inEntity)
        {
            if (IsSimulationRunning)
            {
                simulation.RemoveEntity(inEntity);
            }
            else
            {
                entities.Remove(inEntity);
                if (inEntity is Entities.ReferenceObject)
                {
                    referenceObjects.Remove(inEntity as Entities.ReferenceObject);
                }
            }
        }

        public Entities.Entity FindEntity(Guid instanceGuid)
        {
            return Entities.FirstOrDefault(e => e.InstanceGuid == instanceGuid);
        }

        public long GenerateDeterministicId(Entities.Entity entity)
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(Frosty.Hash.Fnv1.HashString(entity.GetType().Name));
                writer.Write(referenceObjects.Count);
                writer.Write(entities.Count);

                return (long)Frosty.Hash.Murmur2.Hash64(writer.ToByteArray(), 0xDEADBEEF);
            }
        }
    }

    public interface ISimulationUpdateProvider
    {
        event EventHandler OnSimulationUpdated;
    }

    public static class LevelEditorAttachedBehaviors
    {
        public static DependencyProperty LoadedCommandProperty = DependencyProperty.RegisterAttached("LoadedCommand", typeof(ICommand), typeof(LevelEditorAttachedBehaviors), new PropertyMetadata(null, OnLoadedCommandChanged));
        public static DependencyProperty UnloadedCommandProperty = DependencyProperty.RegisterAttached("UnloadedCommand", typeof(ICommand), typeof(LevelEditorAttachedBehaviors), new PropertyMetadata(null, OnUnloadedCommandChanged));
        public static DependencyProperty HideCommandProperty = DependencyProperty.RegisterAttached("HideCommand", typeof(ICommand), typeof(LevelEditorAttachedBehaviors), new PropertyMetadata(null, OnHideCommandChanged));
        public static DependencyProperty ShowCommandProperty = DependencyProperty.RegisterAttached("ShowCommand", typeof(ICommand), typeof(LevelEditorAttachedBehaviors), new PropertyMetadata(null, OnShowCommandChanged));
        public static DependencyProperty SelectedItemChangedCommandProperty = DependencyProperty.RegisterAttached("SelectedItemChangedCommand", typeof(ICommand), typeof(LevelEditorAttachedBehaviors), new PropertyMetadata(null, OnSelectedItemChangedCommandChanged));

        private static void OnLoadedCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                frameworkElement.Loaded
                  += (o, args) =>
                  {
                      (e.NewValue as ICommand).Execute(frameworkElement);
                  };
            }
        }

        public static ICommand GetLoadedCommand(DependencyObject depObj)
        {
            return (ICommand)depObj.GetValue(LoadedCommandProperty);
        }

        public static void SetLoadedCommand(DependencyObject depObj, ICommand value)
        {
            depObj.SetValue(LoadedCommandProperty, value);
        }

        private static void OnUnloadedCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                frameworkElement.Unloaded
                  += (o, args) =>
                  {
                      (e.NewValue as ICommand).Execute(frameworkElement);
                  };
            }
        }

        public static ICommand GetUnloadedCommand(DependencyObject depObj)
        {
            return (ICommand)depObj.GetValue(UnloadedCommandProperty);
        }

        public static void SetUnloadedCommand(DependencyObject depObj, ICommand value)
        {
            depObj.SetValue(UnloadedCommandProperty, value);
        }

        private static void OnHideCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                var parent = VisualTreeHelper.GetParent(frameworkElement);
                while (parent != null && !(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent != null)
                {
                    var window = parent as Window;
                    window.IsVisibleChanged += (o, args) =>
                    {
                        if (!(o as Window).IsVisible)
                            (e.NewValue as ICommand).Execute(frameworkElement);
                    };
                }
            }
        }

        public static ICommand GetHideCommand(DependencyObject depObj)
        {
            return (ICommand)depObj.GetValue(HideCommandProperty);
        }

        public static void SetHideCommand(DependencyObject depObj, ICommand value)
        {
            depObj.SetValue(HideCommandProperty, value);
        }

        private static void OnShowCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                var parent = VisualTreeHelper.GetParent(frameworkElement);
                while (parent != null && !(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent != null)
                {
                    var window = parent as Window;
                    window.IsVisibleChanged += (o, args) =>
                    {
                        if ((o as Window).IsVisible)
                            (e.NewValue as ICommand).Execute(frameworkElement);
                    };
                }
            }
        }

        public static ICommand GetShowCommand(DependencyObject depObj)
        {
            return (ICommand)depObj.GetValue(ShowCommandProperty);
        }

        public static void SetShowCommand(DependencyObject depObj, ICommand value)
        {
            depObj.SetValue(ShowCommandProperty, value);
        }

        public static ICommand GetSelectedItemChangedCommand(DependencyObject depObj)
        {
            return (ICommand)depObj.GetValue(SelectedItemChangedCommandProperty);
        }
        public static void SetSelectedItemChangedCommand(DependencyObject depObj, ICommand value)
        {
            depObj.SetValue(SelectedItemChangedCommandProperty, value);
        }
        private static void OnSelectedItemChangedCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                if (frameworkElement is TreeView)
                {
                    var treeView = frameworkElement as TreeView;
                    treeView.SelectedItemChanged += (o, args) =>
                    {
                        (e.NewValue as ICommand).Execute(args.NewValue);
                    };
                }
            }
        }
    }

    public class PropertyGridModifiedEventArgs : EventArgs
    {
        public ItemModifiedEventArgs Args { get; private set; }
        public bool IsUndoAction { get; private set; }

        public PropertyGridModifiedEventArgs(ItemModifiedEventArgs inArgs, bool isUndoAction)
        {
            Args = inArgs;
            IsUndoAction = isUndoAction;
        }
    }

    public class SchematicsViewModel : IDockableItem, INotifyPropertyChanged, IDragDropTargetProvider
    {
        public string Header => "Schematics";
        public string UniqueId => "UID_LevelEditor_Schematics";
        public string Icon => "Images/Schematics.png";
        public DockManager DockManager => dockManager;
        public ICommand InitializeCommand => new RelayCommand(Initialize);
        public ICommand UnloadCommand => new RelayCommand(Unload);
        public ICommand HideCommand => new RelayCommand(Hide);
        public ICommand ShowCommand => new RelayCommand(Show);
        public ICommand SelectedNodeChangedCommand => new RelayCommand(SelectedNodeChanged);
        public ICommand WireAddedCommand => new RelayCommand(ConnectionAdded);
        public ICommand NodeModifiedCommand => new RelayCommand(NodeModified);
        public ICommand DataModifiedCommand => new RelayCommand(PropertyGridDataModified);
        public IEnumerable<ToolbarItem> ToolbarItems => toolbarItems;
        public IEnumerable<Entities.Entity> Entities => entities;
        public SchematicsData SchematicsData => schematicsData;
        public bool IsDockedInEditor => isDockedInEditor;
        public bool IsGridVisible => isGridVisible;
        public bool IsConnectorOrdersVisible => isConnectorOrdersVisible;
        public bool SuppressLayoutSave => suppressLayoutSave;
        public object ModifiedData => modifiedData;
        public object UpdateDebugLayer => updateDebug;

        private RoutedCommand undoCommand;
        private ToolbarAssetEditor owner;
        private DockManager dockManager;
        private List<ToolbarItem> toolbarItems;
        private List<Entities.Entity> entities;
        private SceneLayer layer;
        private SchematicsData schematicsData;

        private bool isDockedInEditor;
        private bool isGridVisible;
        private bool isConnectorOrdersVisible;
        private bool suppressLayoutSave;

        private object modifiedData;
        private object updateDebug;

        public event EventHandler<SelectedEntityChangedEventArgs> SelectedEntityChanged;
        public event EventHandler<SelectedObjectChangedEventArgs> SelectedObjectChanged;

        public SchematicsViewModel(ToolbarAssetEditor inOwner, SceneLayer inLayer, bool isEditorView = false)
        {
            owner = inOwner;

            isDockedInEditor = isEditorView;
            isGridVisible = true;
            isConnectorOrdersVisible = false;

            dockManager = new DockManager(inOwner);
            if (!isEditorView)
            {
                dockManager.LoadFromConfig("Schematics", new DockManager.DockManagerConfigData()
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
                            UniqueId = "UID_LevelEditor_Toolbox",
                            IsVisible = false,
                            IsSelected = false,
                            Location = DockLocation.BottomLeft
                        }
                    }
                });

                toolbarItems = RegisterToolbarItems();
                SetLayer(inLayer);

                if (inOwner is LevelEditor)
                {
                    (inOwner as LevelEditor).SelectedLayerChanged += OnSelectedLayerChanged;
                }
            }

            if (inOwner is ISimulationUpdateProvider)
            {
                (inOwner as ISimulationUpdateProvider).OnSimulationUpdated += OnSimulationUpdated;
            }

            //var mainWindowType = Assembly.GetEntryAssembly().GetType("FrostyEditor.MainWindow");
            //var project = (FrostyProject)ReflectionUtils.GetPrivateField(mainWindowType, Application.Current.MainWindow, "project");

            // @todo: figure out why above method was returning null
            FrostyProject project = ((dynamic)Application.Current.MainWindow).Project;

            SchematicsLayoutManager.Instance.LoadProjectLayouts(project.Filename);
        }

        public void SetLayer(SceneLayer inLayer)
        {
            if (inLayer != layer)
            {
                layer = inLayer;
                if (layer != null)
                {
                    entities = new List<Entities.Entity>();
                    layer.CollectLogicEntities(entities);

                    FrostySdk.Ebx.Blueprint blueprint = null;
                    Guid blueprintGuid = Guid.Empty;
                    Entities.InterfaceDescriptor interfaceDescriptor = null;

                    if (layer.Entity is Entities.LogicPrefabReferenceObject)
                    {
                        var logicPrefabEntity = layer.Entity as Entities.LogicPrefabReferenceObject;
                        blueprint = logicPrefabEntity.Blueprint.Data;
                        blueprintGuid = logicPrefabEntity.Blueprint.FileGuid;
                        interfaceDescriptor = logicPrefabEntity.InterfaceDescriptor;
                    }
                    else
                    {
                        var refObjEntity = layer.Entity as Entities.ReferenceObject;
                        blueprint = refObjEntity.Blueprint.Data;
                        blueprintGuid = refObjEntity.Blueprint.FileGuid;
                        interfaceDescriptor = refObjEntity.InterfaceDescriptor;
                    }

                    schematicsData = new SchematicsData()
                    {
                        Entities = new ObservableCollection<Entities.Entity>(),
                        LinkConnections = new ObservableCollection<object>(),
                        EventConnections = new ObservableCollection<object>(),
                        PropertyConnections = new ObservableCollection<object>(),
                        InterfaceDescriptor = interfaceDescriptor,
                        BlueprintGuid = blueprintGuid,
                        World = layer.Entity.World
                    };
                    foreach (var entity in entities)
                    {
                        schematicsData.Entities.Add(entity);
                    }
                    foreach (var connection in blueprint.LinkConnections)
                    {
                        schematicsData.LinkConnections.Add(connection);
                    }
                    foreach (var connection in blueprint.EventConnections)
                    {
                        schematicsData.EventConnections.Add(connection);
                    }
                    foreach (var connection in blueprint.PropertyConnections)
                    {
                        if (!Utils.IsFieldProperty(connection.SourceField))
                        {
                            schematicsData.PropertyConnections.Add(connection);
                        }
                    }

                    NotifyPropertyChanged("SchematicsData");
                }
            }
        }

        public List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new DockingToolbarItem("", "Show/Hide properties tab", "Images/Properties.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new PropertiesViewModel(this))), DockManager, "UID_LevelEditor_Properties"),
                new DockingToolbarItem("", "Show/Hide toolbox", "Images/Toolbox.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new SchematicsToolboxViewModel())), DockManager, "UID_LevelEditor_Toolbox"),
                new DividerToolbarItem(),
                new ToggleToolbarItem("", "Show/Hide the grid", "FrostyEditor/Images/Grid.png", true, new RelayCommand((o) => { isGridVisible = !isGridVisible; NotifyPropertyChanged("IsGridVisible"); (o as ToggleToolbarItem).IsToggled = isGridVisible; })),
                new ToggleToolbarItem("", "Show/Hide connector order", "LevelEditorPlugin/Images/ConnectorOrder.png", false, new RelayCommand((o) => { isConnectorOrdersVisible = !isConnectorOrdersVisible; NotifyPropertyChanged("IsConnectorOrdersVisible"); })),
                new RegularToolbarItem("", "Clear the current layout", "LevelEditorPlugin/Images/ClearLayout.png", new RelayCommand((o) => { SchematicsLayoutManager.Instance.ClearLayout(schematicsData.BlueprintGuid); suppressLayoutSave = true; NotifyPropertyChanged("SuppressLayoutSave"); }, (o) => { return (schematicsData != null) ? SchematicsLayoutManager.Instance.GetLayout(schematicsData.BlueprintGuid) != null : false; })),
                new DividerToolbarItem()
            };
        }

        public void Initialize(object obj)
        {
            if (!isDockedInEditor)
            {
                DockManager.AddItemOnLoad(new PropertiesViewModel(this));
                DockManager.AddItemOnLoad(new SchematicsToolboxViewModel());

                undoCommand = new RoutedCommand();
                undoCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));

                (obj as FrameworkElement).CommandBindings.Add(new CommandBinding(undoCommand, Schematics_Undo));
            }
            SelectedNodeChanged(null);
        }

        public void Unload(object obj)
        {
            if (!isDockedInEditor)
            {
                if (owner is LevelEditor)
                {
                    (owner as LevelEditor).SelectedLayerChanged -= OnSelectedLayerChanged;
                }
                DockManager.SaveToConfig("Schematics");
            }
            DockManager.Shutdown();
        }

        public void Hide(object obj)
        {
            DockManager.HideFloatingWindows();
        }

        public void Show(object obj)
        {
            DockManager.ShowFloatingWindows();
        }

        public void SelectNode(object data)
        {
            SelectedNodeChanged(data);
        }

        private void OnSimulationUpdated(object sender, EventArgs e)
        {
            UpdateDebug();
        }

        public void UpdateDebug()
        {
            updateDebug = true;
            NotifyPropertyChanged("UpdateDebugLayer");
            updateDebug = null;
            NotifyPropertyChanged("UpdateDebugLayer");
        }

        public bool IsValidDropSource(IDataObject draggedData)
        {
            if (draggedData.GetDataPresent(typeof(SchematicsDropData)))
                return true;

            if (draggedData.GetDataPresent(typeof(DataExplorerDropData)))
            {
                var dropData = (DataExplorerDropData)draggedData.GetData(typeof(DataExplorerDropData));
                if (TypeLibrary.IsSubClassOf(dropData.Entry.Type, "LogicPrefabBlueprint"))
                    return true;
            }

            return false;
        }

        public void ProcessDrop(IDataObject draggedData, Point dropPoint)
        {
#if MASS_EFFECT
            if (draggedData.GetDataPresent(typeof(UIWidgetDropData)))
            {
                var data = draggedData.GetData(typeof(SchematicsDropData)) as SchematicsDropData;
                var dataType = data.DataType.GetCustomAttribute<Entities.EntityBindingAttribute>().DataType;

                var entity = (Entities.Entity)Activator.CreateInstance(data.DataType, new object[] { CreateEntityData(dataType), null });
                if (entity != null)
                {
                    entity.SetDefaultValues();
                    entity.UserData = dropPoint;

                    // add entity to blueprint
                    var refObjEntity = layer.Entity as Entities.ReferenceObject;

                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add Schematics Node",
                        (o) =>
                        {
                            // add entity to ebx
                            owner.Asset.AddObject(entity.GetRawData());

                            // add to reference
                            refObjEntity.AddEntity(entity);
                            LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);

                            // add to canvas
                            schematicsData.Entities.Add(entity);

                        },
                        (o) =>
                        {
                            // remove from reference
                            refObjEntity.RemoveEntity(entity);

                            // remove from ebx
                            owner.Asset.RemoveObject(entity.GetRawData());
                            LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);

                            // remove from canvas
                            schematicsData.Entities.Remove(schematicsData.Entities.First(e => e.InstanceGuid == entity.InstanceGuid));

                        }));
                }
            }
            else
#endif
            if (draggedData.GetDataPresent(typeof(DataExplorerDropData)))
            {
                var dropData = (DataExplorerDropData)draggedData.GetData(typeof(DataExplorerDropData));
                var asset = App.AssetManager.GetEbx(dropData.Entry);

                var entityData = (FrostySdk.Ebx.LogicReferenceObjectData)CreateEntityData(typeof(FrostySdk.Ebx.LogicPrefabReferenceObjectData));
                entityData.Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = dropData.Entry.Guid, ClassGuid = asset.RootInstanceGuid });

                var entity = (Entities.LogicPrefabReferenceObject)Activator.CreateInstance(typeof(Entities.LogicPrefabReferenceObject), new object[] { entityData, null });

                if (entity != null)
                {
                    entity.SetDefaultValues();
                    entity.UserData = dropPoint;

                    // add entity to blueprint
                    var refObjEntity = layer.Entity as Entities.ReferenceObject;

                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add UIWidget",
                        (o) =>
                        {
                            // add entity to ebx
                            owner.Asset.AddObject(entity.GetRawData());

                            // add to reference
                            refObjEntity.AddEntity(entity);
                            LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);

                            // add to canvas
                            schematicsData.Entities.Add(entity);

                        },
                        (o) =>
                        {
                            // remove from reference
                            refObjEntity.RemoveEntity(entity);

                            // remove from ebx
                            owner.Asset.RemoveObject(entity.GetRawData());
                            LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);

                            // remove from canvas
                            schematicsData.Entities.Remove(schematicsData.Entities.First(e => e.InstanceGuid == entity.InstanceGuid));

                        }));
                }
            }
        }

        private void OnSelectedLayerChanged(object sender, SelectedLayerChangedEventArgs e)
        {
            SetLayer(e.NewSelection);
        }

        private void SelectedNodeChanged(object obj)
        {
            Entities.Entity selectedEntity = obj as Entities.Entity;
            if (selectedEntity == null)
            {
                if (obj == null)
                {
                    if (layer.Entity is Entities.LogicPrefabReferenceObject)
                    {
                        var logicPrefabEntity = layer.Entity as Entities.LogicPrefabReferenceObject;
                        SelectedObjectChanged?.Invoke(this, new SelectedObjectChangedEventArgs(logicPrefabEntity.Blueprint.Data, null));
                    }
                    else if (layer.Entity is Entities.ReferenceObject)
                    {
                        var refObjEntity = layer.Entity as Entities.ReferenceObject;
                        SelectedObjectChanged?.Invoke(this, new SelectedObjectChangedEventArgs(refObjEntity.Blueprint.Data, null));
                    }
                }
                else
                {
                    SelectedObjectChanged?.Invoke(this, new SelectedObjectChangedEventArgs(obj, null));
                }
                return;
            }

            SelectedEntityChanged?.Invoke(this, new SelectedEntityChangedEventArgs(selectedEntity, null));
        }

        private void ConnectionAdded(object obj)
        {
            var e = obj as WireAddedEventArgs;
            var refObjEntity = layer.Entity as Entities.ReferenceObject;
            
            object source = (e.SourceEntity != null) ? (e.SourceEntity as Entities.Entity).GetRawData() : refObjEntity.InterfaceDescriptor.Data;
            object target = (e.TargetEntity != null) ? (e.TargetEntity as Entities.Entity).GetRawData() : refObjEntity.InterfaceDescriptor.Data;

            if (e.ConnectionType == 0)
            {
                // Links
                var connection = new FrostySdk.Ebx.LinkConnection()
                {
                    Source = new FrostySdk.Ebx.PointerRef(source),
                    SourceFieldId = e.SourceId,
                    Target = new FrostySdk.Ebx.PointerRef(target),
                    TargetFieldId = e.TargetId
                };
                refObjEntity.Blueprint.Data.LinkConnections.Add(connection);
                schematicsData.LinkConnections.Add(connection);
            }
            else if (e.ConnectionType == 1)
            {
                // Events
                var connection = new FrostySdk.Ebx.EventConnection()
                {
                    Source = new FrostySdk.Ebx.PointerRef(source),
                    SourceEvent = new FrostySdk.Ebx.EventSpec() { Id = e.SourceId },
                    Target = new FrostySdk.Ebx.PointerRef(target),
                    TargetEvent = new FrostySdk.Ebx.EventSpec() { Id = e.TargetId }
                };

                if (e.SourceEntity == null) connection.TargetType = FrostySdk.Ebx.EventConnectionTargetType.EventConnectionTargetType_ClientAndServer;
                else if (e.SourceEntity.Realm == FrostySdk.Ebx.Realm.Realm_Client)
                {
                    if (e.TargetEntity == null) connection.TargetType = FrostySdk.Ebx.EventConnectionTargetType.EventConnectionTargetType_ClientAndServer;
                    else
                    {
                        switch (e.TargetEntity.Realm)
                        {
                            case FrostySdk.Ebx.Realm.Realm_Client: connection.TargetType = FrostySdk.Ebx.EventConnectionTargetType.EventConnectionTargetType_Client; break;
                            case FrostySdk.Ebx.Realm.Realm_Server: connection.TargetType = FrostySdk.Ebx.EventConnectionTargetType.EventConnectionTargetType_NetworkedClient; break;
                            case FrostySdk.Ebx.Realm.Realm_ClientAndServer: connection.TargetType = FrostySdk.Ebx.EventConnectionTargetType.EventConnectionTargetType_ClientAndServer; break;
                        }
                    }
                }

                refObjEntity.Blueprint.Data.EventConnections.Add(connection);
                schematicsData.EventConnections.Add(connection);
            }
            else
            {
                // Properties
                var connection = new FrostySdk.Ebx.PropertyConnection()
                {
                    Source = new FrostySdk.Ebx.PointerRef(source),
                    SourceFieldId = e.SourceId,
                    Target = new FrostySdk.Ebx.PointerRef(target),
                    TargetFieldId = e.TargetId
                };

                // @todo: prefabs and schematic channels
                if (e.SourceEntity == null) connection.Flags = 18;
                else if (e.SourceEntity.Realm == FrostySdk.Ebx.Realm.Realm_Client)
                {
                    if (e.TargetEntity == null) connection.Flags = 18;
                    else
                    {
                        switch (e.TargetEntity.Realm)
                        {
                            case FrostySdk.Ebx.Realm.Realm_Client: connection.Flags = 2; break;
                            case FrostySdk.Ebx.Realm.Realm_ClientAndServer: connection.Flags = 18; break;
                        }
                    }
                }
                else if (e.SourceEntity.Realm == FrostySdk.Ebx.Realm.Realm_Server)
                {
                    if (e.TargetEntity == null) connection.Flags = 19;
                    else
                    {
                        switch (e.TargetEntity.Realm)
                        {
                            case FrostySdk.Ebx.Realm.Realm_Server: connection.Flags = 3; break;
                            case FrostySdk.Ebx.Realm.Realm_ClientAndServer: connection.Flags = 19; break;
                        }
                    }
                }
                else
                {
                    if (e.TargetEntity == null) connection.Flags = 1;
                    else
                    {
                        switch (e.TargetEntity.Realm)
                        {
                            case FrostySdk.Ebx.Realm.Realm_Client: connection.Flags = 2; break;
                            case FrostySdk.Ebx.Realm.Realm_Server: connection.Flags = 3; break;
                            case FrostySdk.Ebx.Realm.Realm_ClientAndServer: connection.Flags = 1; break;
                        }
                    }
                }

                refObjEntity.Blueprint.Data.PropertyConnections.Add(connection);
                schematicsData.PropertyConnections.Add(connection);
            }

            if (e.TargetEntity != null)
            {
                if (e.ConnectionType == 2)
                {
                    e.TargetEntity.FlagsPropertyRealm = (byte)(e.TargetEntity.Realm + 1);
                    // @todo: prefabs/schematic channels
                }
                else if (e.ConnectionType == 1)
                {
                    e.TargetEntity.FlagsEventRealm = (byte)(e.TargetEntity.Realm + 1);
                    // @todo: prefabs/schematic channels
                }
                else
                {
                    e.SourceEntity.FlagsLinkRealm = (byte)(e.SourceEntity.FlagsLinkRealm | 0x30);
                    e.TargetEntity.FlagsLinkRealm = (byte)(e.TargetEntity.Realm + 1);
                    // @todo: prefabs/schematic channels
                }
            }
        }

        private void NodeModified(object obj)
        {
            var e = obj as NodeModifiedEventArgs;

            // @todo
        }

        private void Schematics_Undo(object sender, RoutedEventArgs e)
        {
            UndoManager.Instance.Undo();
        }

        private void PropertyGridDataModified(object obj)
        {
            var args = obj as PropertyGridModifiedEventArgs;
            if (args.IsUndoAction)
            {
                LoadedAssetManager.Instance.UndoUpdate((layer.Entity as Entities.ReferenceObject).Blueprint);
            }
            else
            {
                LoadedAssetManager.Instance.UpdateAsset((layer.Entity as Entities.ReferenceObject).Blueprint);
            }

            modifiedData = obj;
            NotifyPropertyChanged("ModifiedData");
            modifiedData = null;
            NotifyPropertyChanged("ModifiedData");
        }

        private object CreateEntityData(Type entityDataType)
        {
            FrostySdk.Ebx.DataBusPeer entityData = (FrostySdk.Ebx.DataBusPeer)Activator.CreateInstance(entityDataType);

            Guid instanceGuid = FrostySdk.Utils.GenerateDeterministicGuid(owner.Asset.Objects, entityDataType.Name, owner.Asset.FileGuid);
            entityData.SetInstanceGuid(new FrostySdk.Ebx.AssetClassGuid(instanceGuid, -1));

            byte[] array = instanceGuid.ToByteArray();
            uint flagsValue = (uint)((int)(array[3] & 0x01) << 24 | (int)array[2] << 16 | (int)array[1] << 8 | (int)array[0]);

            entityData.Flags = flagsValue;
            return entityData;
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
