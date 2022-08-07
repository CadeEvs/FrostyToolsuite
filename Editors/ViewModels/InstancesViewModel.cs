using Frosty.Core;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frosty.Core.Managers;

namespace LevelEditorPlugin.Editors
{
    public class InstancesViewModel : Controls.IDockableItem, INotifyPropertyChanged
    {
        public string Header => "Instances";
        public string UniqueId => "UID_LevelEditor_Instances";
        public string Icon => "Images/Instances.png";
        public IEnumerable<Entities.Entity> Instances
        {
            get => instances;
            set
            {
                if (instances != value)
                {
                    instances = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ICommand EntityDoubleClickedCommand { get; private set; }
        public Entities.Entity SelectedEntity
        {
            get => selectedEntity;
            set
            {
                if (selectedEntity != value)
                {
                    var prevSelection = selectedEntity;
                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Select Entity", (o) => { selectedEntity = value; SelectedEntityChanged(selectedEntity); }, (o) => { selectedEntity = prevSelection; SelectedEntityChanged(selectedEntity); NotifyPropertyChanged("SelectedEntity"); }));
                    NotifyPropertyChanged();
                }
            }
        }

        private IEditorProvider owner;
        private Entities.Entity selectedEntity;
        private IEnumerable<Entities.Entity> instances;

        public InstancesViewModel(IEditorProvider inOwner, Entities.Entity currentSelection)
        {
            owner = inOwner;
            owner.Screen.SelectedEntityChanged += SelectedEntityChangedFromScreen;

            List<Layers.SceneLayer> layers = new List<Layers.SceneLayer>();
            owner.RootLayer.CollectLayers(layers);

            EntityDoubleClickedCommand = new RelayCommand(EntityDoubleClicked);
            selectedEntity = currentSelection;

            foreach (var layer in layers)
                layer.VisibilityChanged += SceneLayerVisibilityChanged;

            UpdateEntityInstances();
        }

        private void SelectedEntityChangedFromScreen(object sender, Screens.SelectedEntityChangedEventArgs e)
        {
            SelectedEntity = e.Entity;
        }

        private void UpdateEntityInstances()
        {
            List<Entities.Entity> entities = new List<Entities.Entity>();
            owner.RootLayer.CollectEntities(entities);

            Instances = entities;
        }

        private void SceneLayerVisibilityChanged(object sender, Layers.LayerVisibilityChangedEventArgs e)
        {
            UpdateEntityInstances();
        }

        private void EntityDoubleClicked(object state)
        {
            SelectedEntity = state as Entities.Entity;
            owner.CenterOnSelection();
        }

        private void SelectedEntityChanged(Entities.Entity newSelection)
        {
            owner.SelectEntity(newSelection);
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
