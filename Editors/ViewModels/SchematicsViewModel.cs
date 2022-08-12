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
using FrostySdk.Ebx;
using LevelEditorPlugin.Entities;
using Entity = LevelEditorPlugin.Entities.Entity;
using Window = System.Windows.Window;

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
                TaskCompletionSource<Dispatcher> dispatcherReady = new TaskCompletionSource<Dispatcher>();
                Thread dispatcherThread = new Thread(() =>
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
            foreach (Entity entity in Entities)
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
            FrameworkElement frameworkElement = depObj as FrameworkElement;
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
            FrameworkElement frameworkElement = depObj as FrameworkElement;
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
            FrameworkElement frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(frameworkElement);
                while (parent != null && !(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent != null)
                {
                    Window window = parent as Window;
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
            FrameworkElement frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(frameworkElement);
                while (parent != null && !(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent != null)
                {
                    Window window = parent as Window;
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
            FrameworkElement frameworkElement = depObj as FrameworkElement;
            if (frameworkElement != null && e.NewValue is ICommand)
            {
                if (frameworkElement is TreeView)
                {
                    TreeView treeView = frameworkElement as TreeView;
                    treeView.SelectedItemChanged += (o, args) =>
                    {
                        (e.NewValue as ICommand).Execute(args.NewValue);
                    };
                }
            }
        }
    }

    public class RemoveNodeConnectionsUndoUnit : IUndoUnit
    {
        public string Text => "Delete Node Connections";
        
        private Dictionary<PropertyConnection, int> propertyConnectionsToRemove = new Dictionary<PropertyConnection, int>();
        private Dictionary<EventConnection, int> eventConnectionsToRemove = new Dictionary<EventConnection, int>();
        private Dictionary<LinkConnection, int> linkConnectionsToRemove = new Dictionary<LinkConnection, int>();
        
        private Entity entity;
        private Blueprint blueprint;
        private SchematicsData data;
        
        public RemoveNodeConnectionsUndoUnit(Entity inEntity, Blueprint inBlueprint, SchematicsData inData)
        {
            entity = inEntity;
            blueprint = inBlueprint;
            data = inData;
            
            //
            // Find all connections that entity is connected to
            //
            
            // property
            for (int i = 0; i < blueprint.PropertyConnections.Count; i++)
            {
                PropertyConnection connection = blueprint.PropertyConnections[i];
                if (connection.Source.Internal == entity.GetRawData())
                {
                    propertyConnectionsToRemove.Add(connection, i);
                }
                else if (connection.Target.Internal == entity.GetRawData())
                {
                    propertyConnectionsToRemove.Add(connection, i);
                }
            }
            // event
            for (int i = 0; i < blueprint.EventConnections.Count; i++)
            {
                EventConnection connection = blueprint.EventConnections[i];
                if (connection.Source.Internal == entity.GetRawData())
                {
                    eventConnectionsToRemove.Add(connection, i);
                }
                else if (connection.Target.Internal == entity.GetRawData())
                {
                    eventConnectionsToRemove.Add(connection, i);
                }
            }
            // link
            for (int i = 0; i < blueprint.LinkConnections.Count; i++)
            {
                LinkConnection connection = blueprint.LinkConnections[i];
                if (connection.Source.Internal == entity.GetRawData())
                {
                    linkConnectionsToRemove.Add(connection, i);
                }
                else if (connection.Target.Internal == entity.GetRawData())
                {
                    linkConnectionsToRemove.Add(connection, i);
                }
            }
        }
        
        public void Do()
        {
            //
            // Remove connections from blueprint and SchematicsData
            // Store the index the connection was removed from so we can properly re-insert it on undo
            //
            
            // property
            foreach (KeyValuePair<PropertyConnection, int> connection in propertyConnectionsToRemove)
            {
                blueprint.PropertyConnections.Remove(connection.Key);
                data.PropertyConnections.Remove(connection.Key);
            }
            // event
            foreach (KeyValuePair<EventConnection, int> connection in eventConnectionsToRemove)
            {
                blueprint.EventConnections.Remove(connection.Key);
                data.EventConnections.Remove(connection.Key);
            }
            // link
            foreach (KeyValuePair<LinkConnection, int> connection in linkConnectionsToRemove)
            {
                blueprint.LinkConnections.Remove(connection.Key);
                data.LinkConnections.Remove(connection.Key);
            }
        }

        public void Undo()
        {
            //
            // Re-insert connections to blueprint and SchematicsData
            //
            
            // property
            foreach (KeyValuePair<PropertyConnection, int> connection in propertyConnectionsToRemove)
            {
                blueprint.PropertyConnections.Insert(connection.Value, connection.Key);
                data.PropertyConnections.Insert(connection.Value, connection.Key);
            }
            // event
            foreach (KeyValuePair<EventConnection, int> connection in eventConnectionsToRemove)
            {
                blueprint.EventConnections.Insert(connection.Value, connection.Key);
                data.EventConnections.Insert(connection.Value, connection.Key);
            }
            // link
            foreach (KeyValuePair<LinkConnection, int> connection in linkConnectionsToRemove)
            {
                blueprint.LinkConnections.Insert(connection.Value, connection.Key);
                data.LinkConnections.Insert(connection.Value, connection.Key);
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
        public DockManager DockManager => m_dockManager;
        public ICommand InitializeCommand => new RelayCommand(Initialize);
        public ICommand UnloadCommand => new RelayCommand(Unload);
        public ICommand HideCommand => new RelayCommand(Hide);
        public ICommand ShowCommand => new RelayCommand(Show);
        public ICommand SelectedNodeChangedCommand => new RelayCommand(SelectedNodeChanged);
        public ICommand WireAddedCommand => new RelayCommand(ConnectionAdded);
        public ICommand WireRemovedCommand => new RelayCommand(ConnectionRemoved);
        public ICommand NodeRemovedCommand => new RelayCommand(NodeRemoved);
        public ICommand NodeModifiedCommand => new RelayCommand(NodeModified);
        public ICommand DataModifiedCommand => new RelayCommand(PropertyGridDataModified);
        public IEnumerable<ToolbarItem> ToolbarItems => m_toolbarItems;
        public IEnumerable<Entities.Entity> Entities => m_entities;
        public SchematicsData SchematicsData => m_schematicsData;
        public bool IsDockedInEditor => m_isDockedInEditor;
        public bool IsGridVisible => m_isGridVisible;
        public bool IsConnectorOrdersVisible => m_isConnectorOrdersVisible;
        public bool SuppressLayoutSave => m_suppressLayoutSave;
        public object ModifiedData => m_modifiedData;
        public object UpdateDebugLayer => m_updateDebug;

        private RoutedCommand m_undoCommand;
        private ToolbarAssetEditor m_owner;
        private DockManager m_dockManager;
        private List<ToolbarItem> m_toolbarItems;
        private List<Entities.Entity> m_entities;
        private SceneLayer m_layer;
        private SchematicsData m_schematicsData;

        private bool m_isDockedInEditor;
        private bool m_isGridVisible;
        private bool m_isConnectorOrdersVisible;
        private bool m_suppressLayoutSave;

        private object m_modifiedData;
        private object m_updateDebug;

        public event EventHandler<SelectedEntityChangedEventArgs> SelectedEntityChanged;
        public event EventHandler<SelectedObjectChangedEventArgs> SelectedObjectChanged;

        public SchematicsViewModel(ToolbarAssetEditor inOwner, SceneLayer inLayer, bool isEditorView = false)
        {
            m_owner = inOwner;

            m_isDockedInEditor = isEditorView;
            m_isGridVisible = true;
            m_isConnectorOrdersVisible = false;

            m_dockManager = new DockManager(inOwner);
            if (!isEditorView)
            {
                m_dockManager.LoadFromConfig("Schematics", new DockManager.DockManagerConfigData()
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

                m_toolbarItems = RegisterToolbarItems();
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
            if (inLayer != m_layer)
            {
                m_layer = inLayer;
                if (m_layer != null)
                {
                    m_entities = new List<Entities.Entity>();
                    m_layer.CollectLogicEntities(m_entities);

                    FrostySdk.Ebx.Blueprint blueprint = null;
                    Guid blueprintGuid = Guid.Empty;
                    Entities.InterfaceDescriptor interfaceDescriptor = null;

                    if (m_layer.Entity is Entities.LogicPrefabReferenceObject)
                    {
                        LogicPrefabReferenceObject logicPrefabEntity = m_layer.Entity as Entities.LogicPrefabReferenceObject;
                        blueprint = logicPrefabEntity.Blueprint.Data;
                        blueprintGuid = logicPrefabEntity.Blueprint.FileGuid;
                        interfaceDescriptor = logicPrefabEntity.InterfaceDescriptor;
                    }
                    else
                    {
                        ReferenceObject refObjEntity = m_layer.Entity as Entities.ReferenceObject;
                        blueprint = refObjEntity.Blueprint.Data;
                        blueprintGuid = refObjEntity.Blueprint.FileGuid;
                        interfaceDescriptor = refObjEntity.InterfaceDescriptor;
                    }

                    m_schematicsData = new SchematicsData()
                    {
                        Entities = new ObservableCollection<Entities.Entity>(),
                        LinkConnections = new ObservableCollection<object>(),
                        EventConnections = new ObservableCollection<object>(),
                        PropertyConnections = new ObservableCollection<object>(),
                        InterfaceDescriptor = interfaceDescriptor,
                        BlueprintGuid = blueprintGuid,
                        World = m_layer.Entity.World
                    };
                    foreach (Entity entity in m_entities)
                    {
                        m_schematicsData.Entities.Add(entity);
                    }
                    foreach (LinkConnection connection in blueprint.LinkConnections)
                    {
                        m_schematicsData.LinkConnections.Add(connection);
                    }
                    foreach (EventConnection connection in blueprint.EventConnections)
                    {
                        m_schematicsData.EventConnections.Add(connection);
                    }
                    foreach (PropertyConnection connection in blueprint.PropertyConnections)
                    {
                        if (!Utils.IsFieldProperty(connection.SourceField))
                        {
                            m_schematicsData.PropertyConnections.Add(connection);
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
                new ToggleToolbarItem("", "Show/Hide the grid", "FrostyEditor/Images/Grid.png", true, new RelayCommand((o) => { m_isGridVisible = !m_isGridVisible; NotifyPropertyChanged("IsGridVisible"); (o as ToggleToolbarItem).IsToggled = m_isGridVisible; })),
                new ToggleToolbarItem("", "Show/Hide connector order", "LevelEditorPlugin/Images/ConnectorOrder.png", false, new RelayCommand((o) => { m_isConnectorOrdersVisible = !m_isConnectorOrdersVisible; NotifyPropertyChanged("IsConnectorOrdersVisible"); })),
                new RegularToolbarItem("", "Clear the current layout", "LevelEditorPlugin/Images/ClearLayout.png", new RelayCommand((o) => { SchematicsLayoutManager.Instance.ClearLayout(m_schematicsData.BlueprintGuid); m_suppressLayoutSave = true; NotifyPropertyChanged("SuppressLayoutSave"); }, (o) => { return (m_schematicsData != null) ? SchematicsLayoutManager.Instance.GetLayout(m_schematicsData.BlueprintGuid) != null : false; })),
                new DividerToolbarItem()
            };
        }

        public void Initialize(object obj)
        {
            if (!m_isDockedInEditor)
            {
                DockManager.AddItemOnLoad(new PropertiesViewModel(this));
                DockManager.AddItemOnLoad(new SchematicsToolboxViewModel());

                m_undoCommand = new RoutedCommand();
                m_undoCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));

                (obj as FrameworkElement).CommandBindings.Add(new CommandBinding(m_undoCommand, Schematics_Undo));
            }
            SelectedNodeChanged(null);
        }

        public void Unload(object obj)
        {
            if (!m_isDockedInEditor)
            {
                if (m_owner is LevelEditor)
                {
                    (m_owner as LevelEditor).SelectedLayerChanged -= OnSelectedLayerChanged;
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
            m_updateDebug = true;
            NotifyPropertyChanged("UpdateDebugLayer");
            m_updateDebug = null;
            NotifyPropertyChanged("UpdateDebugLayer");
        }

        public bool IsValidDropSource(IDataObject draggedData)
        {
            if (draggedData.GetDataPresent(typeof(SchematicsDropData)))
                return true;

            if (draggedData.GetDataPresent(typeof(DataExplorerDropData)))
            {
                DataExplorerDropData dropData = (DataExplorerDropData)draggedData.GetData(typeof(DataExplorerDropData));
                if (TypeLibrary.IsSubClassOf(dropData.Entry.Type, "LogicPrefabBlueprint"))
                    return true;
            }

            return false;
        }

        public void ProcessDrop(IDataObject draggedData, Point dropPoint)
        {
            if (draggedData.GetDataPresent(typeof(SchematicsDropData)))
            {
                SchematicsDropData data = draggedData.GetData(typeof(SchematicsDropData)) as SchematicsDropData;
                Type dataType = data.DataType.GetCustomAttribute<Entities.EntityBindingAttribute>().DataType;

                Entity entity = (Entities.Entity)Activator.CreateInstance(data.DataType, new object[] { CreateEntityData(dataType), null });
                if (entity != null)
                {
                    entity.SetDefaultValues();
                    entity.UserData = dropPoint;

                    // add entity to blueprint
                    ReferenceObject refObjEntity = m_layer.Entity as Entities.ReferenceObject;

                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add Schematics Node",
                        (o) =>
                        {
                            // add entity to ebx
                            m_owner.Asset.AddObject(entity.GetRawData());

                            // add to reference
                            refObjEntity.AddEntity(entity);
                            LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);

                            // add to canvas
                            m_schematicsData.Entities.Add(entity);

                        },
                        (o) =>
                        {
                            // remove from reference
                            refObjEntity.RemoveEntity(entity);

                            // remove from ebx
                            m_owner.Asset.RemoveObject(entity.GetRawData());
                            LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);

                            // remove from canvas
                            m_schematicsData.Entities.Remove(m_schematicsData.Entities.First(e => e.InstanceGuid == entity.InstanceGuid));

                        }));
                }
            }
            else if (draggedData.GetDataPresent(typeof(DataExplorerDropData)))
            {
                DataExplorerDropData dropData = (DataExplorerDropData)draggedData.GetData(typeof(DataExplorerDropData));
                EbxAsset asset = App.AssetManager.GetEbx(dropData.Entry);

                LogicReferenceObjectData entityData = (FrostySdk.Ebx.LogicReferenceObjectData)CreateEntityData(typeof(FrostySdk.Ebx.LogicPrefabReferenceObjectData));
                entityData.Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = dropData.Entry.Guid, ClassGuid = asset.RootInstanceGuid });

                LogicPrefabReferenceObject entity = (Entities.LogicPrefabReferenceObject)Activator.CreateInstance(typeof(Entities.LogicPrefabReferenceObject), new object[] { entityData, null });

                if (entity != null)
                {
                    entity.SetDefaultValues();
                    entity.UserData = dropPoint;

                    // add entity to blueprint
                    ReferenceObject refObjEntity = m_layer.Entity as Entities.ReferenceObject;

                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Add UIWidget",
                        (o) =>
                        {
                            // add entity to ebx
                            m_owner.Asset.AddObject(entity.GetRawData());

                            // add to reference
                            refObjEntity.AddEntity(entity);
                            LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);

                            // add to canvas
                            m_schematicsData.Entities.Add(entity);

                        },
                        (o) =>
                        {
                            // remove from reference
                            refObjEntity.RemoveEntity(entity);

                            // remove from ebx
                            m_owner.Asset.RemoveObject(entity.GetRawData());
                            LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);

                            // remove from canvas
                            m_schematicsData.Entities.Remove(m_schematicsData.Entities.First(e => e.InstanceGuid == entity.InstanceGuid));

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
                    if (m_layer.Entity is Entities.LogicPrefabReferenceObject)
                    {
                        LogicPrefabReferenceObject logicPrefabEntity = m_layer.Entity as Entities.LogicPrefabReferenceObject;
                        SelectedObjectChanged?.Invoke(this, new SelectedObjectChangedEventArgs(logicPrefabEntity.Blueprint.Data, null));
                    }
                    else if (m_layer.Entity is Entities.ReferenceObject)
                    {
                        ReferenceObject refObjEntity = m_layer.Entity as Entities.ReferenceObject;
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
            WireAddedEventArgs e = obj as WireAddedEventArgs;
            ReferenceObject refObjEntity = m_layer.Entity as Entities.ReferenceObject;

            object source = (e.SourceEntity != null) ? (e.SourceEntity as Entities.Entity).GetRawData() : refObjEntity.InterfaceDescriptor.Data;
            object target = (e.TargetEntity != null) ? (e.TargetEntity as Entities.Entity).GetRawData() : refObjEntity.InterfaceDescriptor.Data;
            
            byte origSourceFlagsRealm = 0;
            byte origTargetFlagsRealm = 0;
            uint origSourceFlags = 0;
            uint origTargetFlags = 0;
            
            // copy original flags if node (not interface)
            if (source is DataBusPeer)
            {
                DataBusPeer entity = source as DataBusPeer;
                origSourceFlags = entity.Flags;
            }
            if (target is DataBusPeer)
            {
                DataBusPeer entity = target as DataBusPeer;
                origTargetFlags = entity.Flags;
            }
            
            // we need to create a container to separate the undo into two steps: creating the connection and editing the entity flag
            UndoContainer container = new UndoContainer("Create Connection");

            // create and add connection based on type
            dynamic schematicDataConnections = null;
            dynamic blueprintConnections = null; 
            dynamic newConnection = null;
            if (e.ConnectionType == 0) // Links
            {
                LinkConnection connection = new FrostySdk.Ebx.LinkConnection()
                {
                    Source = new FrostySdk.Ebx.PointerRef(source),
                    SourceFieldId = e.SourceId,
                    Target = new FrostySdk.Ebx.PointerRef(target),
                    TargetFieldId = e.TargetId
                };

                // store original flags realm for undo unit
                if (e.SourceEntity != null)
                    origSourceFlagsRealm = e.SourceEntity.FlagsLinkRealm;
                if (e.TargetEntity != null)
                    origTargetFlagsRealm = e.TargetEntity.FlagsLinkRealm;
                
                blueprintConnections = refObjEntity.Blueprint.Data.LinkConnections;
                schematicDataConnections = m_schematicsData.LinkConnections;
                newConnection = connection;
            }
            else if (e.ConnectionType == 1) // Events
            {
                EventConnection connection = new FrostySdk.Ebx.EventConnection()
                {
                    Source = new FrostySdk.Ebx.PointerRef(source),
                    SourceEvent = new FrostySdk.Ebx.EventSpec() { Id = e.SourceId },
                    Target = new FrostySdk.Ebx.PointerRef(target),
                    TargetEvent = new FrostySdk.Ebx.EventSpec() { Id = e.TargetId }
                };

                // store original flags realm for undo unit
                if (e.SourceEntity != null)
                    origSourceFlagsRealm = e.SourceEntity.FlagsEventRealm;
                if (e.TargetEntity != null)
                    origTargetFlagsRealm = e.TargetEntity.FlagsEventRealm;
                
                // update target type
                if (e.SourceEntity == null)
                {
                    connection.TargetType = FrostySdk.Ebx.EventConnectionTargetType.EventConnectionTargetType_ClientAndServer;
                }
                else if (e.SourceEntity.Realm == FrostySdk.Ebx.Realm.Realm_Client)
                {
                    if (e.TargetEntity == null)
                    {
                        connection.TargetType = FrostySdk.Ebx.EventConnectionTargetType.EventConnectionTargetType_ClientAndServer;
                    }
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

                blueprintConnections = refObjEntity.Blueprint.Data.EventConnections;
                schematicDataConnections = m_schematicsData.EventConnections;
                newConnection = connection;
            }
            else if (e.ConnectionType == 2) // Properties
            {
                PropertyConnection connection = new FrostySdk.Ebx.PropertyConnection()
                {
                    Source = new FrostySdk.Ebx.PointerRef(source),
                    SourceFieldId = e.SourceId,
                    Target = new FrostySdk.Ebx.PointerRef(target),
                    TargetFieldId = e.TargetId
                };

                // store original flags realm for undo unit
                if (e.SourceEntity != null)
                    origSourceFlagsRealm = e.SourceEntity.FlagsPropertyRealm;
                if (e.TargetEntity != null)
                    origTargetFlagsRealm = e.TargetEntity.FlagsPropertyRealm;
                
                // update flags
                // @todo: prefabs and schematic channels
                if (e.SourceEntity == null)
                {
                    connection.Flags = 18;
                }
                else if (e.SourceEntity.Realm == FrostySdk.Ebx.Realm.Realm_Client)
                {
                    if (e.TargetEntity == null)
                    {
                        connection.Flags = 18;
                    }
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
                    if (e.TargetEntity == null)
                    {
                        connection.Flags = 19;
                    }
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
                    if (e.TargetEntity == null)
                    {
                        connection.Flags = 1;
                    }
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

                blueprintConnections = refObjEntity.Blueprint.Data.PropertyConnections;
                schematicDataConnections = m_schematicsData.PropertyConnections;
                newConnection = connection;
            }

            container.Add(new GenericUndoUnit("", 
                (o) =>
                    {
                        blueprintConnections.Add(newConnection);
                        schematicDataConnections.Add(newConnection);
                    },
                    (o) =>
                    {
                        blueprintConnections.Remove(newConnection);
                        schematicDataConnections.Remove(newConnection);
                    }));

            // update entity realms
            if (e.TargetEntity != null)
            {
                container.Add(new GenericUndoUnit("",
                    (o) =>
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
                            if (e.SourceEntity != null)
                                e.SourceEntity.FlagsLinkRealm = (byte)(e.SourceEntity.FlagsLinkRealm | 0x30);
                            
                            e.TargetEntity.FlagsLinkRealm = (byte)(e.TargetEntity.Realm + 1);
                            // @todo: prefabs/schematic channels
                        }
                    },
                    (o) =>
                    {
                        if (e.ConnectionType == 2)
                        {
                            e.TargetEntity.FlagsPropertyRealm = origTargetFlagsRealm;
                        }
                        else if (e.ConnectionType == 1)
                        {
                            e.TargetEntity.FlagsEventRealm = origTargetFlagsRealm;
                            // @todo: prefabs/schematic channels
                        }
                        else
                        {
                            if (e.SourceEntity != null)
                                e.SourceEntity.FlagsLinkRealm = origSourceFlagsRealm;

                            e.TargetEntity.FlagsLinkRealm = origTargetFlagsRealm;
                            // @todo: prefabs/schematic channels
                        }

                        // set original flags if node (not interface)
                        if (source is DataBusPeer)
                        {
                            DataBusPeer entity = source as DataBusPeer;
                            entity.Flags = origSourceFlags;
                        }
                        if (target is DataBusPeer)
                        {
                            DataBusPeer entity = target as DataBusPeer;
                            entity.Flags = origTargetFlags;
                        }
                    }));
            }

            if (container.HasItems)
            {
                container.Add(new GenericUndoUnit("", o => LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint), o => LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint)));
                UndoManager.Instance.CommitUndo(container);
            }
        }

        private void ConnectionRemoved(object obj)
        {
            WireRemovedEventArgs e = obj as WireRemovedEventArgs;
            ReferenceObject refObjEntity = m_layer.Entity as Entities.ReferenceObject;
            
            // @todo: update flags
            
            // we need to create a container to separate the undo into two steps: deleting the connection and updating the entity flags
            dynamic schematicDataConnections = null;
            dynamic blueprintConnections = null; 
            dynamic connection = null;
            
            if (e.Connection is FrostySdk.Ebx.EventConnection)
            {
                connection = (FrostySdk.Ebx.EventConnection)e.Connection;
                blueprintConnections = refObjEntity.Blueprint.Data.EventConnections;
                schematicDataConnections = m_schematicsData.EventConnections;
            }
            else if (e.Connection is FrostySdk.Ebx.PropertyConnection)
            {
                connection = (FrostySdk.Ebx.PropertyConnection)e.Connection;
                blueprintConnections = refObjEntity.Blueprint.Data.PropertyConnections;
                schematicDataConnections = m_schematicsData.PropertyConnections;
            }
            else if (e.Connection is FrostySdk.Ebx.LinkConnection)
            {
                connection = (FrostySdk.Ebx.LinkConnection)e.Connection;
                blueprintConnections = refObjEntity.Blueprint.Data.LinkConnections;
                schematicDataConnections = m_schematicsData.LinkConnections;
            }
            
            e.Container.Add(new GenericUndoUnit("", 
                (o) =>
                {
                    blueprintConnections.Remove(connection);
                    schematicDataConnections.Remove(connection);
                    
                    // @todo: only update asset if we're removing a base asset connection
                    LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);
                },
                (o) =>
                {
                    blueprintConnections.Add(connection);
                    schematicDataConnections.Add(connection);
                    
                    LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);
                }));
        }
        
        private void NodeRemoved(object obj)
        {
            NodeRemovedEventArgs e = obj as NodeRemovedEventArgs;
            if (e.Entity is Entity entity)
            {
                ReferenceObject refObjEntity = m_layer.Entity as Entities.ReferenceObject;
                
                // @todo: support deleting wirepoints and updating entity flags
                e.Container.Add(new RemoveNodeConnectionsUndoUnit(entity, refObjEntity.Blueprint.Data, m_schematicsData));
                
                e.Container.Add(new GenericUndoUnit("Delete Schematics Node Entity",
                    (o) =>
                    {
                        // remove from reference
                        refObjEntity.RemoveEntity(entity);

                        // remove from ebx
                        m_owner.Asset.RemoveObject(entity.GetRawData());
                        // @todo: only update asset if we're removing a base asset node
                        LoadedAssetManager.Instance.UpdateAsset(refObjEntity.Blueprint);

                        // remove from canvas
                        m_schematicsData.Entities.Remove(m_schematicsData.Entities.First(se => se.InstanceGuid == entity.InstanceGuid));
                    },
                    (o) =>
                    {
                        // add entity to ebx
                        m_owner.Asset.AddObject(entity.GetRawData());

                        // add to reference
                        refObjEntity.AddEntity(entity);
                        LoadedAssetManager.Instance.UndoUpdate(refObjEntity.Blueprint);

                        // add to canvas
                        m_schematicsData.Entities.Add(entity);
                    }));
            }
        }
        
        private void NodeModified(object obj)
        {
            NodeModifiedEventArgs e = obj as NodeModifiedEventArgs;

            // @todo
        }

        private void Schematics_Undo(object sender, RoutedEventArgs e)
        {
            UndoManager.Instance.Undo();
        }

        private void PropertyGridDataModified(object obj)
        {
            PropertyGridModifiedEventArgs args = obj as PropertyGridModifiedEventArgs;
            if (args.IsUndoAction)
            {
                LoadedAssetManager.Instance.UndoUpdate((m_layer.Entity as Entities.ReferenceObject).Blueprint);
            }
            else
            {
                LoadedAssetManager.Instance.UpdateAsset((m_layer.Entity as Entities.ReferenceObject).Blueprint);
            }

            m_modifiedData = obj;
            NotifyPropertyChanged("ModifiedData");
            m_modifiedData = null;
            NotifyPropertyChanged("ModifiedData");
        }

        private object CreateEntityData(Type entityDataType)
        {
            FrostySdk.Ebx.DataBusPeer entityData = (FrostySdk.Ebx.DataBusPeer)Activator.CreateInstance(entityDataType);

            Guid instanceGuid = FrostySdk.Utils.GenerateDeterministicGuid(m_owner.Asset.Objects, entityDataType.Name, m_owner.Asset.FileGuid);
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
