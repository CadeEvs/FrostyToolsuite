using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using LevelEditorPlugin.Controls;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LevelEditorPlugin.Entities;

namespace LevelEditorPlugin.Editors
{
    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyViewport))]
    public class ObjectBlueprintEditor : SpatialEditor
    {
        private const string PART_Renderer = "PART_Renderer";
        private FrostyViewport viewport;

        public ObjectBlueprintEditor(ILogger inLogger)
            : base(inLogger)
        {
            screen = new LevelEditorScreen(true);
            dockManager.LoadFromConfig("ObjectBlueprintEditor", new Controls.DockManager.DockManagerConfigData()
            {
                Layouts = new List<Controls.DockManager.DockLayoutData>()
                {
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Components",
                        IsVisible = true,
                        IsSelected = true,
                        Location = DockLocation.BottomLeft
                    },
                    new DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_Properties",
                        IsVisible = true,
                        IsSelected = true,
                        Location = DockLocation.TopRight
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
                    }
                }
            });
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new DockingToolbarItem("", "Show/Hide components tab", "Images/Components.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new ComponentsViewModel(this))), DockManager, "UID_LevelEditor_Components"),
                new DockingToolbarItem("", "Show/Hide properties tab", "Images/Properties.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new PropertiesViewModel(this, selectedEntity))), DockManager, "UID_LevelEditor_Properties"),
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
            DockManager.SaveToConfig("ObjectBlueprintEditor");
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

                FrostySdk.Ebx.ObjectReferenceObjectData objectData = new FrostySdk.Ebx.ObjectReferenceObjectData()
                {
                    Blueprint = new FrostySdk.Ebx.PointerRef(new EbxImportReference()
                    {
                        FileGuid = Asset.FileGuid,
                        ClassGuid = Asset.RootInstanceGuid
                    })
                };

                world = new EntityWorld();
                ReferenceObject refObj = (Entities.ReferenceObject)Entities.Entity.CreateEntity(objectData, Asset.FileGuid, world);

                editingWorld = refObj;
                rootLayer = MakeFakeLayer();

                viewport.SetPaused(true);
                screen.AddEntity(refObj);
                screen.ShowTaskWindow = true;
                viewport.SetPaused(false);

                editingWorld = refObj;
                currentLoadingState = null;

                world.Initialize();
            });

            timer.Stop();
            logger.Log($"Loaded object blueprint in {timer.Elapsed.ToString()}");

            DockManager.AddItemOnLoad(new ComponentsViewModel(this));
            DockManager.AddItemOnLoad(new PropertiesViewModel(this, editingWorld));
            DockManager.AddItemOnLoad(new SchematicsViewModel(this, rootLayer));
        }
    }
}
