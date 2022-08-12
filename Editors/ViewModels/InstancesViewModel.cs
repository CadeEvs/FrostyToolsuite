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
using LevelEditorPlugin.Entities;
using LevelEditorPlugin.Layers;

namespace LevelEditorPlugin.Editors
{
    public class InstancesViewModel : Controls.IDockableItem, INotifyPropertyChanged
    {
        public string Header => "Instances";
        public string UniqueId => "UID_LevelEditor_Instances";
        public string Icon => "Images/Instances.png";
        public IEnumerable<Entities.Entity> Instances
        {
            get => m_instances;
            set
            {
                if (m_instances != value)
                {
                    m_instances = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ICommand EntityDoubleClickedCommand { get; private set; }
        public Entities.Entity SelectedEntity
        {
            get => m_selectedEntity;
            set
            {
                if (m_selectedEntity != value)
                {
                    Entity prevSelection = m_selectedEntity;
                    UndoManager.Instance.CommitUndo(new GenericUndoUnit("Select Entity", (o) => { m_selectedEntity = value; SelectedEntityChanged(m_selectedEntity); }, (o) => { m_selectedEntity = prevSelection; SelectedEntityChanged(m_selectedEntity); NotifyPropertyChanged("SelectedEntity"); }));
                    NotifyPropertyChanged();
                }
            }
        }

        private IEditorProvider m_owner;
        private Entities.Entity m_selectedEntity;
        private IEnumerable<Entities.Entity> m_instances;

        public InstancesViewModel(IEditorProvider inOwner, Entity inCurrentSelection)
        {
            m_owner = inOwner;
            m_owner.Screen.SelectedEntityChanged += SelectedEntityChangedFromScreen;

            List<Layers.SceneLayer> layers = new List<Layers.SceneLayer>();
            m_owner.RootLayer.CollectLayers(layers);

            EntityDoubleClickedCommand = new RelayCommand(EntityDoubleClicked);
            m_selectedEntity = inCurrentSelection;

            foreach (SceneLayer layer in layers)
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
            m_owner.RootLayer.CollectEntities(entities);

            Instances = entities;
        }

        private void SceneLayerVisibilityChanged(object sender, Layers.LayerVisibilityChangedEventArgs e)
        {
            UpdateEntityInstances();
        }

        private void EntityDoubleClicked(object state)
        {
            SelectedEntity = state as Entities.Entity;
            m_owner.CenterOnSelection();
        }

        private void SelectedEntityChanged(Entities.Entity newSelection)
        {
            m_owner.SelectEntity(newSelection);
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
