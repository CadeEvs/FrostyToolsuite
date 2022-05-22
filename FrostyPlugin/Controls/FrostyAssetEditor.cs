using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Frosty.Core.Controls
{
    public class ToolbarItem
    {
        public string Text { get; private set; }
        public string ToolTip { get; private set; }
        public ImageSource Icon { get; private set; }
        public RelayCommand Command { get; private set; }

        public bool IsAddedByPlugin { get; private set; }
        
        public ToolbarItem(string text, string tooltip, string icon, RelayCommand inCommand, bool isAddedByPlugin = false)
        {
            Text = text;
            ToolTip = tooltip;
            if (!string.IsNullOrEmpty(icon))
                Icon = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/" + icon) as ImageSource;
            Command = inCommand;
            IsAddedByPlugin = isAddedByPlugin;
        }
    }

    public class FrostyAssetEditor : Control
    {
        #region -- Properties --

        #region -- RootObject --
        public object RootObject => asset.RootObject;
        public IEnumerable<object> RootObjects => asset.RootObjects;
        public IEnumerable<object> Objects => asset.Objects;

        #endregion

        #region -- AssetEntry --
        public static readonly DependencyProperty AssetEntryProperty = DependencyProperty.Register("AssetEntry", typeof(AssetEntry), typeof(FrostyAssetEditor), new FrameworkPropertyMetadata(null));
        public AssetEntry AssetEntry
        {
            get => (AssetEntry)GetValue(AssetEntryProperty);
            private set => SetValue(AssetEntryProperty, value);
        }
        #endregion

        #region -- Asset --
        public EbxAsset Asset => asset;

        #endregion

        #region -- AssetDirty --
        public static readonly DependencyProperty AssetModifiedProperty = DependencyProperty.Register("AssetModified", typeof(bool), typeof(FrostyAssetEditor), new UIPropertyMetadata(false, OnAssetModifiedChanged));
        public bool AssetModified
        {
            get => (bool)GetValue(AssetModifiedProperty);
            set => SetValue(AssetModifiedProperty, value);
        }
        private static void OnAssetModifiedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            bool isModified = (bool)e.NewValue;
            if (isModified)
            {
                FrostyAssetEditor editor = o as FrostyAssetEditor;

                editor.asset.Update();
                App.AssetManager.ModifyEbx(editor.AssetEntry.Name, editor.asset);

                // do any extra processing
                editor.InvokeOnAssetModified();
                editor.AssetModified = false;
            }
        }
        #endregion

        #endregion

        protected event RoutedEventHandler onAssetModified;
        public event RoutedEventHandler OnAssetModified
        {
            add => onAssetModified += value;
            remove => onAssetModified -= value;
        }

        protected ILogger logger;
        protected List<object> objects;
        protected List<object> rootObjects;
        protected Dictionary<Guid, EbxAsset> dependentObjects = new Dictionary<Guid, EbxAsset>();
        protected EbxAsset asset;

        static FrostyAssetEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyAssetEditor), new FrameworkPropertyMetadata(typeof(FrostyAssetEditor)));
        }

        public FrostyAssetEditor(ILogger inLogger)
        {
            logger = inLogger;
        }

        public void AddDependentObject(Guid guid)
        {
            if (asset.AddDependency(guid))
            {
                // add new dependent object if its not already in the list
                if (!dependentObjects.ContainsKey(guid))
                    dependentObjects.Add(guid, App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(guid)));
            }
        }

        public virtual EbxAsset RefreshDependentObject(Guid guid)
        {
            if (!dependentObjects.ContainsKey(guid))
                return null;
            dependentObjects[guid] = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(guid));
            return dependentObjects[guid];
        }

        public virtual EbxAsset GetDependentObject(Guid guid)
        {
            if (!dependentObjects.ContainsKey(guid))
                return null;
            return dependentObjects[guid];
        }

        public virtual void AddObject(object obj)
        {
            asset.AddObject(obj);
        }

        public virtual void RemoveObject(object obj)
        {
            asset.RemoveObject(obj);
        }

        public int SetAsset(AssetEntry entry)
        {
            if (entry is EbxAssetEntry)
            {
                FrostyTaskWindow.Show("Opening Asset", "", (task) =>
                {
                    asset = LoadAsset(entry as EbxAssetEntry);

                    int totalCount = asset.Dependencies.Count();
                    int index = 0;

                    task.Update("Loading dependencies");
                    foreach (Guid guid in asset.Dependencies)
                    {
                        EbxAssetEntry dependentEntry = App.AssetManager.GetEbxEntry(guid);

                        task.Update(progress: (index++ / (double)totalCount) * 100.0d);

                        if (dependentEntry != null)
                            dependentObjects.Add(guid, App.AssetManager.GetEbx(dependentEntry));
                    }
                });
            }

            // now set entry
            AssetEntry = entry;
            return 0;
        }

        protected virtual EbxAsset LoadAsset(EbxAssetEntry entry) 
        {
            return App.AssetManager.GetEbx(entry);
        }

        protected virtual void InvokeOnAssetModified()
        {
            onAssetModified?.Invoke(this, new RoutedEventArgs());
        }

        public virtual void Closed()
        {
        }

        public virtual List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>() { new ToolbarItem("View Instances", "View class instances", null, new RelayCommand(ViewInstances_Click, ViewInstances_CanClick)) };
        }

        private void ViewInstances_Click(object state)
        {
            if (!(GetTemplateChild("PART_AssetPropertyGrid") is FrostyPropertyGrid pg))
                return;

            AssetInstancesWindow win = new AssetInstancesWindow(asset.RootObjects, pg.SelectedClass, Asset, (EbxAssetEntry)AssetEntry);
            bool result = win.ShowDialog() == true;

            // regardless of result, process any newly created objects, and any delete requests
            foreach (dynamic obj in win.NewlyCreatedObjects)
            {
                if (TypeLibrary.IsSubClassOf(obj, "Asset"))
                {
                    // set the name to match the name of the root asset
                    obj.Name = AssetEntry.Name;
                }
                asset.AddRootObject(obj);
            }
            foreach (object obj in win.DeletedObjects)
            {
                if (obj == pg.Object)
                    pg.Object = RootObject;
                asset.RemoveObject(obj);
            }

            // now modify asset
            if (win.Modified)
                AssetModified = true;

            if (result)
                pg.SetClass(win.SelectedItem);
        }

        private bool ViewInstances_CanClick(object state) => true;
    }
}
