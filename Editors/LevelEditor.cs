using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Screens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditorPlugin.Entities;

namespace LevelEditorPlugin.Editors
{
    public static class Utils
    {
        private static bool IsTypeValid(Type a, Type b)
        {
            return a == b || a.IsSubclassOf(b);
        }

        // This should only be used to obtain internal references or external IF the asset has already been loaded
        // through the LoadedAssetManager

        public static T GetObjectAs<T>(this PointerRef pr)
        {
            if (pr.Type == PointerRefType.External)
            {
                EbxAsset asset = LoadedAssetManager.Instance.GetEbxAsset(pr);
                Debug.Assert(asset != null, "Asset was not previously loaded via the LoadedAssetManager");

                dynamic obj = asset.GetObject(pr.External.ClassGuid);
                Debug.Assert(!(obj is Asset), "GetObjectAs should not be used on Assets");

                if (IsTypeValid(obj.GetType(), typeof(T)))
                    return (T)obj;
            }
            else if (pr.Type == PointerRefType.Internal)
            {
                if (IsTypeValid(pr.Internal.GetType(), typeof(T)))
                    return (T)pr.Internal;
            }

            return default(T);
        }

        public static Guid GetInstanceGuid(this PointerRef pr)
        {
            if (pr.Type == PointerRefType.Null)
                return Guid.Empty;

            else if (pr.Type == PointerRefType.Internal)
            {
                DataContainer container = pr.Internal as FrostySdk.Ebx.DataContainer;
                if (container.__InstanceGuid.IsExported)
                    return container.__InstanceGuid.ExportedGuid;

                return Guid.Parse(container.__InstanceGuid.ToString());
            }

            return pr.External.ClassGuid;
        }

        public static bool IsFieldProperty(string fieldName)
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
            {
                Type extension = Type.GetType("LevelEditorPlugin.UtilsExtension");
                if (extension != null)
                {
                    return (bool)extension.GetMethod("IsFieldProperty", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { fieldName });
                }
            }
            return false;
        }
    }

    public class SelectedEntityChangedEventArgs : EventArgs
    {
        public Entities.Entity NewSelection { get; private set; }
        public Entities.Entity OldSelection { get; private set; }

        public SelectedEntityChangedEventArgs(Entities.Entity inNew, Entities.Entity inOld)
        {
            NewSelection = inNew;
            OldSelection = inOld;
        }
    }

    public class SelectedLayerChangedEventArgs : EventArgs
    {
        public Layers.SceneLayer NewSelection { get; private set; }
        public Layers.SceneLayer OldSelection { get; private set; }

        public SelectedLayerChangedEventArgs(Layers.SceneLayer inNew, Layers.SceneLayer inOld)
        {
            NewSelection = inNew;
            OldSelection = inOld;
        }
    }

    public class SelectedObjectChangedEventArgs : EventArgs
    {
        public object NewSelection { get; private set; }
        public object OldSelection { get; private set; }

        public SelectedObjectChangedEventArgs(object inNew, object inOld)
        {
            NewSelection = inNew;
            OldSelection = inOld;
        }
    }

    [TemplatePart(Name = PART_Renderer, Type = typeof(FrostyViewport))]
    public class LevelEditor : SpatialEditor
    {
        private const string PART_Renderer = "PART_Renderer";

        private FrostyViewport viewport;

        //static LevelEditor()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(LevelEditor), new FrameworkPropertyMetadata(typeof(LevelEditor)));
        //}

        public LevelEditor(ILogger inLogger)
            : base(inLogger)
        {
            screen = new LevelEditorScreen(false);
            dockManager.LoadFromConfig("LevelEditor", new Controls.DockManager.DockManagerConfigData()
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
                        Location = Controls.DockLocation.Bottom,
                    },
                    new Controls.DockManager.DockLayoutData()
                    {
                        UniqueId = "UID_LevelEditor_TerrainLayers",
                        IsVisible = true,
                        IsSelected = false,
                        Location = Controls.DockLocation.TopLeft
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
                new DockingToolbarItem("", "Show/Hide terrain layers tab", "Images/Terrain.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new TerrainLayersViewModel(this))), DockManager, "UID_LevelEditor_TerrainLayers"),
                new DockingToolbarItem("", "Show/Hide timeline editor", "Images/Timeline.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new TimelineViewModel(this))), DockManager, "UID_LevelEditor_Timeline"),
                new FloatingOnlyDockingToolbarItem("", "Show/Hide schematics editor", "Images/Schematics.png", new RelayCommand((o) => DockManager.AddItem(((DockingToolbarItem)o).Location, new SchematicsViewModel(this, rootLayer))), DockManager, "UID_LevelEditor_Schematics"),
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
            DockManager.SaveToConfig("LevelEditor");
            viewport.Shutdown();

            base.Closed();
        }

        protected override void Initialize()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // first time loading stuff goes here
            FrostyTaskWindow.Show($"Loading {Path.GetFileName(AssetEntry.Name)}", "", (task) =>
            {
                currentLoadingState = new LoadingStateInfo()
                {
                    Task = task,
                    Logger = logger
                };

                world = new EntityWorld();
                WorldReferenceObject refObj = new Entities.WorldReferenceObject(Asset.FileGuid, RootObject as WorldData, asset, world);

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
            logger.Log($"Level loaded in {timer.Elapsed.ToString()}");

            DockManager.AddItemOnLoad(new LayersViewModel(this));
            DockManager.AddItemOnLoad(new InstancesViewModel(this, null));
            DockManager.AddItemOnLoad(new PropertiesViewModel(this, editingWorld));
            DockManager.AddItemOnLoad(new TimelineViewModel(this));
            DockManager.AddItemOnLoad(new TerrainLayersViewModel(this));
            DockManager.AddItemOnLoad(new SchematicsViewModel(this, rootLayer));
        }
    }
}
