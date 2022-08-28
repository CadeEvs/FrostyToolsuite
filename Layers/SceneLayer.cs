using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditorPlugin.Entities;
using SharpDX;

namespace LevelEditorPlugin.Layers
{
    public class LayerVisibilityChangedEventArgs : EventArgs
    {
        public SceneLayer Layer { get; private set; }
        public bool Visibility { get; private set; }

        public LayerVisibilityChangedEventArgs(SceneLayer inLayer, bool newVisibility)
        {
            Layer = inLayer;
            Visibility = newVisibility;
        }
    }

    public class LayerSelectionChangedEventArgs : EventArgs
    {
        public SceneLayer Layer { get; private set; }
        public bool Selected { get; private set; }

        public LayerSelectionChangedEventArgs(SceneLayer inLayer, bool newSelected)
        {
            Layer = inLayer;
            Selected = newSelected;
        }
    }

    public class SceneLayer : INotifyPropertyChanged
    {
        public string LayerName { get; private set; }
        public Entities.Entity Entity { get; private set; }
        public List<SceneLayer> ChildLayers { get; private set; } = new List<SceneLayer>();
        public List<Entities.Entity> Entities { get; private set; } = new List<Entities.Entity>();
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;

                    foreach (SceneLayer layer in ChildLayers)
                        layer.IsVisible = isVisible;

                    foreach (Entity entity in Entities)
                        entity.SetVisibility(isVisible);

                    NotifyPropertyChanged();
                    VisibilityChanged?.Invoke(this, new LayerVisibilityChangedEventArgs(this, isVisible));
                }
            }
        }
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
                    SelectionChanged?.Invoke(this, new LayerSelectionChangedEventArgs(this, IsSelected));
                }
            }
        }
        public Color4 LayerColor => layerColor;

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

        private bool isSelected;
        private bool isExpanded;
        private bool isVisible;
        private Color4 layerColor;

        public SceneLayer(Entities.Entity inEntity, string layerName, Color4? color = null)
        {
            Entity = inEntity;
            LayerName = layerName;
            isVisible = true;
            isExpanded = true;
            isSelected = false;
            layerColor = (color.HasValue) ? color.Value : new Color4(1.0f, 0.0f, 0.0f, 1.0f);
        }

        public void AddEntity(Entities.Entity inEntity)
        {
            inEntity.Layer = this;
            Entities.Add(inEntity);

            if (inEntity is Entities.IComponentEntity)
            {
                AddComponents(inEntity as Entities.IComponentEntity);
            }
        }

        public void CollectEntities(List<Entities.Entity> entities)
        {
            if (isVisible)
            {
                entities.AddRange(Entities.Where(e => (e is Entities.ISpatialEntity || e is Entities.ISpatialReferenceEntity) && !(e is Entities.INotRealSpatialEntity)));
            }

            foreach (SceneLayer layer in ChildLayers)
            {
                layer.CollectEntities(entities);
            }
        }

        // temp function
        public void CollectLogicEntities(List<Entities.Entity> entities)
        {
            entities.AddRange(Entities.Where(e => e is Entities.ILogicEntity));
            entities.AddRange(ChildLayers.Where(l => l.Entity is Entities.SubWorldReferenceObject).Select(l => l.Entity));

            foreach (SceneLayer layer in ChildLayers)
            {
                if (layer.Entity is Entities.LayerReferenceObject)
                {
                    layer.CollectLogicEntities(entities);
                }
            }
        }

        // temp function
        public void CollectComponentEntities(List<Entities.Entity> entities)
        {
            entities.AddRange(Entities.Where(e => e is Entities.IComponentEntity));
            foreach (SceneLayer layer in ChildLayers)
            {
                if (layer.Entity is Entities.LayerReferenceObject)
                {
                    layer.CollectComponentEntities(entities);
                }
            }
        }

        // temp function
        public void CollectTimelines(List<Entities.Entity> timelines)
        {
            timelines.AddRange(Entities.Where(e => e is Entities.TimelineEntity));
            foreach (SceneLayer layer in ChildLayers)
            {
                if (layer.Entity is Entities.LayerReferenceObject)
                {
                    layer.CollectTimelines(timelines);
                }
            }
        }

        public void CollectLayers(List<SceneLayer> layers)
        {
            layers.AddRange(ChildLayers);
            foreach (SceneLayer layer in ChildLayers)
            {
                layer.CollectLayers(layers);
            }
        }

        private void AddComponents(Entities.IComponentEntity componentEntity)
        {
            foreach (Entity component in componentEntity.Components)
            {
                Entities.Add(component);
                if (component is Entities.IComponentEntity)
                {
                    AddComponents(component as Entities.IComponentEntity);
                }
            }
        }

        public event EventHandler<LayerVisibilityChangedEventArgs> VisibilityChanged;
        public event EventHandler<LayerSelectionChangedEventArgs> SelectionChanged;
    }
}
