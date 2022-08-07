using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LevelEditorPlugin.Entities;

namespace LevelEditorPlugin.Editors
{
    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyViewport))]
    public class SpatialPrefabEditor : SpatialEditor
    {
        private const string PART_Renderer = "PART_Renderer";

        private FrostyViewport viewport;

        public SpatialPrefabEditor(ILogger inLogger)
            : base(inLogger)
        {
            screen = new LevelEditorScreen(true);
            dockManager.LoadFromConfig("SpatialPrefabEditor", new Controls.DockManager.DockManagerConfigData()
            {
                Layouts = new List<Controls.DockManager.DockLayoutData>()
                {
                    new Controls.DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Layers",
                        IsVisible = true,
                        IsSelected = true,
                        Location = Controls.DockLocation.TopLeft
                    },
                    new Controls.DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Instances",
                        IsVisible = true,
                        IsSelected = true,
                        Location = Controls.DockLocation.BottomLeft
                    },
                    new Controls.DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Properties",
                        IsVisible = true,
                        IsSelected = true,
                        Location = Controls.DockLocation.TopRight
                    },
                    new Controls.DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Timeline",
                        IsVisible = false,
                        IsSelected = false,
                        Location = Controls.DockLocation.Bottom
                    },
                    new Controls.DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Schematics",
                        IsVisible = false,
                        IsSelected = false,
                        Location = Controls.DockLocation.Floating,
                        FloatingData = new Controls.DockManager.DockLayoutFloatingData()
                        {
                             Width = 800,
                             Height = 400
                        }
                    }
                }
            });
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new DockingToolbarItem("", "Show/Hide layers tab", "Images/Layers.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new LayersViewModel(this))), DockManager, "UID_LevelEditor_Layers"),
                new DockingToolbarItem("", "Show/Hide instances tab", "Images/Instances.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new InstancesViewModel(this, selectedEntity))), DockManager, "UID_LevelEditor_Instances"),
                new DockingToolbarItem("", "Show/Hide properties tab", "Images/Properties.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new PropertiesViewModel(this, selectedEntity))), DockManager, "UID_LevelEditor_Properties"),
                new DockingToolbarItem("", "Show/Hide timeline editor", "Images/Timeline.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new TimelineViewModel(this))), DockManager, "UID_LevelEditor_Timeline"),
                new FloatingOnlyDockingToolbarItem("", "Show/Hide schematics editor", "Images/Schematics.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new SchematicsViewModel(this, rootLayer))), DockManager, "UID_LevelEditor_Schematics"),
                new DividerToolbarItem(),
                new RegularToolbarItem("", "Capture thumbnail preview", "LevelEditorPlugin/Images/CaptureThumbnail.png", new RelayCommand((o) => { CaptureThumbnail(viewport); })),
                new ToggleToolbarItem("", "Show/Hide thumbnail safezone", "LevelEditorPlugin/Images/ThumbnailSafezone.png", false, new RelayCommand((o) => { ShowThumbnailSafeZone(thumbnailBorder.Visibility == Visibility.Collapsed, viewport); }))
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            viewport = GetTemplateChild(PART_Renderer) as FrostyViewport;
            viewport.Screen = screen;
        }

        public override void Closed()
        {
            DockManager.SaveToConfig("SpatialPrefabEditor");
            viewport.Shutdown();

            base.Closed();
        }

        protected override void Initialize()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            FrostyTaskWindow.Show($"Loading {Path.GetFileName(AssetEntry.Name)}", "", (task) =>
            {
                currentLoadingState = new LoadingStateInfo()
                {
                    Task = task,
                    Logger = logger
                };

                FrostySdk.Ebx.SpatialPrefabReferenceObjectData objectData = new FrostySdk.Ebx.SpatialPrefabReferenceObjectData()
                {
                    Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference()
                    {
                        FileGuid = Asset.FileGuid,
                        ClassGuid = Asset.RootInstanceGuid
                    })
                };

                world = new EntityWorld();
                SpatialPrefabReferenceObject refObj = (Entities.SpatialPrefabReferenceObject)Entities.Entity.CreateEntity(objectData, Asset.FileGuid, world);

                rootLayer = refObj.GetLayer();

                viewport.SetPaused(true);
                screen.AddEntity(refObj);
                screen.ShowTaskWindow = true;
                viewport.SetPaused(false);

                editingWorld = refObj;
                currentLoadingState = null;

                world.Initialize();
            });

            timer.Stop();
            logger.Log($"Loaded spatial prefab in {timer.Elapsed.ToString()}");

            DockManager.AddItemOnLoad(new LayersViewModel(this));
            DockManager.AddItemOnLoad(new InstancesViewModel(this, null));
            DockManager.AddItemOnLoad(new PropertiesViewModel(this, editingWorld));
            DockManager.AddItemOnLoad(new TimelineViewModel(this));
            DockManager.AddItemOnLoad(new SchematicsViewModel(this, rootLayer));
        }
    }
}
