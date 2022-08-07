using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using LevelEditorPlugin.Controls;
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LevelEditorPlugin.Editors
{
    class LogicPrefabEditor : LogicEditor, ISchematicsInterfaceProvider, ISimulationUpdateProvider
    {
        public static readonly DependencyProperty IsInGameViewProperty = DependencyProperty.Register("IsInGameView", typeof(bool), typeof(LogicPrefabEditor), new FrameworkPropertyMetadata(false));

        public bool IsInGameView
        {
            get => (bool)GetValue(IsInGameViewProperty);
            set => SetValue(IsInGameViewProperty, value);
        }

        public InterfaceDescriptor Interface => entity.InterfaceDescriptor;

        public event EventHandler<InterfaceOutputPropertyChangedEventArgs> OnInterfaceOutputPropertyChanged;
        public event EventHandler<InterfaceOutputEventTriggeredEventArgs> OnInterfaceOutputEventTriggered;
        public event EventHandler OnSimulationUpdated;

        public LogicPrefabEditor(ILogger inLogger)
            : base(inLogger)
        {
            viewModel.DockManager.LoadFromConfig("LogicPrefabEditor", new DockManager.DockManagerConfigData()
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
                        UniqueId = "UID_LevelEditor_Toolbox",
                        IsVisible = false,
                        IsSelected = false,
                        Location = DockLocation.BottomLeft
                    }
                }
            });
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            var items = base.RegisterToolbarItems();
            items.Add(new DockingToolbarItem("", "Show/Hide interface tab", "Images/Interface.png", new RelayCommand((o) => ViewModel.DockManager.AddItem(((DockingToolbarItem)o).Location, new InterfaceViewModel(this))), ViewModel.DockManager, "UID_LevelEditor_Interface"));
            items.Add(new DividerToolbarItem());
            items.Add(new ToggleToolbarItem("", "Enter/Exit simulation", "LevelEditorPlugin/Images/GameView.png", false, new RelayCommand((o) => { IsInGameView = !IsInGameView; (o as ToggleToolbarItem).IsToggled = IsInGameView; OnIsInGameViewChanged(); })));
            return items;
        }

        protected override void Initialize()
        {
            FrostySdk.Ebx.LogicPrefabReferenceObjectData objectData = new FrostySdk.Ebx.LogicPrefabReferenceObjectData()
            {
                Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = Asset.FileGuid, ClassGuid = Asset.RootInstanceGuid })
            };

            world = new EntityWorld();
            world.OnSimulationUpdated += SimulationUpdated;

            entity = Entity.CreateEntity(objectData, Asset.FileGuid, world) as LogicPrefabReferenceObject;
            rootLayer = MakeFakeLayer();

            world.Initialize();

            if (entity.InterfaceDescriptor != null)
            {
                entity.InterfaceDescriptor.OnInterfaceOutputPropertyChanged += InterfaceOutputPropertyChanged;
                entity.InterfaceDescriptor.OnInterfaceOutputEventTriggered += InterfaceOutputEventTriggered;
            }

            viewModel.DockManager.AddItemOnLoad(new PropertiesViewModel(viewModel));
            viewModel.DockManager.AddItemOnLoad(new InterfaceViewModel(this));
            viewModel.DockManager.AddItemOnLoad(new SchematicsToolboxViewModel());

            base.Initialize();
        }

        public override void Closed()
        {
            viewModel.DockManager.SaveToConfig("LogicPrefabEditor");
            world.Shutdown();

            base.Closed();
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

        private void SimulationUpdated(object sender, EventArgs e)
        {
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
    }
}
