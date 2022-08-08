using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetEntityData))]
	public class UIWidgetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIWidgetEntityData>, IComponentEntity, IHideInSchematicsView, IContainerOfEntities
	{
		public new FrostySdk.Ebx.UIWidgetEntityData Data => data as FrostySdk.Ebx.UIWidgetEntityData;
		public IEnumerable<Entity> Components => components;
        public IEnumerable<UIElementLayerEntity> Layers => layers;
        public Rect ClipRect => new Rect(0, 0, Data.Size.X, Data.Size.Y);
		public override string DisplayName => "UIWidget";
        public virtual IEnumerable<ConnectionDesc> ChildProperties => new List<ConnectionDesc>();

		protected List<Entity> components = new List<Entity>();
        protected List<UIElementLayerEntity> layers = new List<UIElementLayerEntity>();

        protected List<Assets.UITextureMappingAsset> textureMappings = new List<Assets.UITextureMappingAsset>();

		public UIWidgetEntity(FrostySdk.Ebx.UIWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            foreach (var objRef in Data.TextureMappings)
            {
                textureMappings.Add(LoadedAssetManager.Instance.LoadAsset<Assets.UITextureMappingAsset>(this, objRef));
            }
		}

        public void AddEntity(Entity inEntity)
        {
            inEntity.SetParent(this);
            Data.Components.Add(new FrostySdk.Ebx.PointerRef(inEntity.GetRawData()));
        }

        public void RemoveEntity(Entity inEntity)
        {
            Guid instanceGuid = inEntity.InstanceGuid;
            Data.Components.Remove(Data.Components.Find(p => p.GetInstanceGuid() == instanceGuid));
        }

        public override void BeginSimulation()
        {
            base.BeginSimulation();

            foreach (var component in components)
            {
                if (component is ISchematicsType)
                {
                    var schematicsEntity = component as ISchematicsType;
                    schematicsEntity.BeginSimulation();
                }
            }
            foreach (var layer in layers)
            {
                layer.BeginSimulation();
            }
        }

        public override void Update_PreFrame()
        {
            base.Update_PreFrame();
            foreach (var component in components)
            {
                if (component is ISchematicsType)
                {
                    var schematicsEntity = component as ISchematicsType;
                    schematicsEntity.Update_PreFrame();
                }
            }
            foreach (var layer in layers)
            {
                layer.Update_PreFrame();
            }
        }

        public override void Update_PostFrame()
        {
            base.Update_PostFrame();
            foreach (var component in components)
            {
                if (component is ISchematicsType)
                {
                    var schematicsEntity = component as ISchematicsType;
                    schematicsEntity.Update_PostFrame();
                }
            }
            foreach (var layer in layers)
            {
                layer.Update_PostFrame();
            }
        }

        public System.Windows.Media.Imaging.BitmapImage GetTexture(int id, out Point4D uvRect)
        {
            foreach (var textureMapping in textureMappings)
            {
                var bitmap = textureMapping.GetTexture(id);
                if (bitmap != null)
                {
                    uvRect = textureMapping.GetUVRect(id);
                    return bitmap;
                }
            }
            uvRect = new Point4D();
            return null;
        }

		public void SpawnComponents()
		{
            foreach (var objRef in Data.Layers)
            {
                var layerElementData = objRef.GetObjectAs<FrostySdk.Ebx.UIElementLayerEntityData>();
                var layerEntity = CreateEntity(layerElementData) as UIElementLayerEntity;

                if (layerEntity != null)
                {
                    layers.Add(layerEntity);
                }
            }

            foreach (var objPointer in Data.Components)
            {
                if (objPointer.Type == FrostySdk.IO.PointerRefType.External)
                {
                    // make sure the external asset is already loaded and add a ref to it
                    //additionalAssets.Add(LoadedAssetManager.Instance.LoadAsset<Assets.Asset>(objPointer.External.FileGuid));
                    System.Diagnostics.Debug.Assert(false);
                }

                var objectData = objPointer.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                var component = CreateEntity(objectData);

                if (component != null)
                {
                    components.Add(component);
                    if (component is Component)
                    {
                        var gameComponent = component as Component;
                        gameComponent.SpawnComponents();
                    }
                }
            }
        }

        public Entity FindEntity(Guid instanceGuid)
        {
            foreach (var layer in layers)
            {
                if (layer.InstanceGuid == instanceGuid)
                    return layer;

                if (layer is IContainerOfEntities)
                {
                    var containerEntity = layer as IContainerOfEntities;
                    var entity = containerEntity.FindEntity(instanceGuid);
                    if (entity != null)
                        return entity;
                }
            }

            foreach (var component in components)
            {
                if (component.InstanceGuid == instanceGuid)
                    return component;

                if (component is IContainerOfEntities)
                {
                    var containerEntity = component as IContainerOfEntities;
                    var entity = containerEntity.FindEntity(instanceGuid);
                    if (entity != null)
                        return entity;
                }
            }

            return null;
        }

        public override void Destroy()
        {
            foreach (var mapping in textureMappings)
            {
                LoadedAssetManager.Instance.UnloadAsset(mapping);
            }
            foreach (var layer in layers)
            {
                layer.Destroy();
            }
			foreach (var component in components)
			{
				component.Destroy();
			}

            base.Destroy();
        }

        public virtual void PerformLayout()
        {
            foreach (var layer in Layers)
            {
                layer.PerformLayout();
            }
        }

        public virtual void OnMouseMove(Point mousePos)
        {
            foreach (var layer in Layers)
            {
                layer.OnMouseMove(mousePos);
            }
        }

        public virtual void OnMouseDown(Point mousePos, MouseButton button)
        {
            foreach (var layer in Layers)
            {
                layer.OnMouseDown(mousePos, button);
            }
        }

        public virtual void OnMouseUp(Point mousePos, MouseButton button)
        {
            foreach (var layer in Layers)
            {
                layer.OnMouseUp(mousePos, button);
            }
        }
    }
}

