using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Editors
{
    public class ComponentSelectionChangedEventArgs : EventArgs
    {
        public Entities.Entity Entity { get; private set; }
        public bool Selected { get; private set; }

        public ComponentSelectionChangedEventArgs(Entities.Entity inEntity, bool newSelected)
        {
            Entity = inEntity;
            Selected = newSelected;
        }
    }

    public class ComponentWrapper : INotifyPropertyChanged
    {
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                    SelectionChanged.Invoke(this, new ComponentSelectionChangedEventArgs(entity, IsSelected));
                }
            }
        }
        public IEnumerable<ComponentWrapper> Children => children;
        public string DisplayName => entity.DisplayName.EndsWith("Component") ? entity.DisplayName.Replace("Component", "") : entity.DisplayName;

        private bool isSelected;
        private bool isExpanded;
        private Entities.Entity entity;
        private List<ComponentWrapper> children = new List<ComponentWrapper>();

        public ComponentWrapper(Entities.Entity inEntity)
        {
            entity = inEntity;

            var componentEntity = entity as Entities.IComponentEntity;
            foreach (var child in componentEntity.Components)
            {
                if (child is Entities.IComponentEntity)
                {
                    children.Add(new ComponentWrapper(child));
                }
            }
        }

        public event EventHandler<ComponentSelectionChangedEventArgs> SelectionChanged;

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

    public class ComponentsViewModel : Controls.IDockableItem
    {
        public string Header => "Components";
        public string UniqueId => "UID_LevelEditor_Components";
        public string Icon => "Images/Components.png";
        public IEnumerable<ComponentWrapper> Components => components;

        private IEditorProvider owner;
        private List<ComponentWrapper> components = new List<ComponentWrapper>();

        public ComponentsViewModel(IEditorProvider inOwner)
        {
            owner = inOwner;

            var layer = owner.RootLayer;
            var rootEntity = layer.Entities[0];

            components.Add(new ComponentWrapper(rootEntity));
            RecursiveSetSelectionChanged(components[0]);
        }

        private void RecursiveSetSelectionChanged(ComponentWrapper wrapper)
        {
            wrapper.SelectionChanged += SelectedComponentChanged;
            foreach (var child in wrapper.Children)
                RecursiveSetSelectionChanged(child);
        }

        private void SelectedComponentChanged(object sender, ComponentSelectionChangedEventArgs e)
        {
            if (e.Selected)
                owner.SelectEntity(e.Entity);
        }
    }
}
