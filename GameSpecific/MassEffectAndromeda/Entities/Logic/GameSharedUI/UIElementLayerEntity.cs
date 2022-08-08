using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementLayerEntityData))]
	public class UIElementLayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIElementLayerEntityData>, IHideInSchematicsView, IContainerOfEntities
	{
		protected readonly int Property_Visible = Frosty.Hash.Fnv1.HashString("Visible");

		public new FrostySdk.Ebx.UIElementLayerEntityData Data => data as FrostySdk.Ebx.UIElementLayerEntityData;
		public override string DisplayName => "UIElementLayer";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Visible", Direction.In, typeof(bool))
			};
        }
        public IEnumerable<Entity> Elements => elements;
		public bool Visible
        {
			get
            {
				bool visible = visibleProperty.Value;

				if (parent is UIWidgetEntity)
				{
					var widget = (parent as UIWidgetEntity);
					visible &= (widget.Parent as UIElementWidgetReferenceEntity).Visible;
				}

				return visible;
			}
        }
		public float Alpha
        {
			get
            {
				return ((parent as UIWidgetEntity).Parent as UIElementWidgetReferenceEntity).Alpha;
            }
        }

		protected List<Entity> elements = new List<Entity>();
		protected Property<bool> visibleProperty;

		public UIElementLayerEntity(FrostySdk.Ebx.UIElementLayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			foreach (var objRef in Data.Elements)
			{
				var objectData = objRef.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
				var element = CreateEntity(objectData);

				if (element != null)
				{
					elements.Add(element);
				}
			}

			visibleProperty = new Property<bool>(this, Property_Visible, true);
		}

		public void AddEntity(Entity inEntity)
		{
			inEntity.SetParent(this);
			Data.Elements.Add(new FrostySdk.Ebx.PointerRef(inEntity.GetRawData()));
		}

		public void RemoveEntity(Entity inEntity)
		{
			Guid instanceGuid = inEntity.InstanceGuid;
			Data.Elements.Remove(Data.Elements.Find(p => p.GetInstanceGuid() == instanceGuid));
		}

		public override void BeginSimulation()
		{
			base.BeginSimulation();
			foreach (var element in Elements)
			{
				if (element is ISchematicsType)
				{
					(element as ISchematicsType).BeginSimulation();
				}
			}
		}

		public override void EndSimulation()
		{
			base.EndSimulation();
			foreach (var element in Elements)
			{
				if (element is ISchematicsType)
				{
					(element as ISchematicsType).EndSimulation();
				}
			}
		}

		public override void Update_PreFrame()
        {
            base.Update_PreFrame();
			foreach (var element in elements)
			{
				if (element is ISchematicsType)
				{
					var schematicsEntity = element as ISchematicsType;
					schematicsEntity.Update_PreFrame();
				}
			}
		}

		public override void Update_PostFrame()
		{
			base.Update_PostFrame();
			foreach (var element in elements)
			{
				if (element is ISchematicsType)
				{
					var schematicsEntity = element as ISchematicsType;
					schematicsEntity.Update_PostFrame();
				}
			}
		}

		public Entity FindEntity(Guid instanceGuid)
		{
			foreach (var element in elements)
			{
				if (element.InstanceGuid == instanceGuid)
					return element;

				if (element is IContainerOfEntities)
				{
					var containerEntity = element as IContainerOfEntities;
					var entity = containerEntity.FindEntity(instanceGuid);
					if (entity != null)
						return entity;
				}
			}

			return null;
		}

		public void PerformLayout()
		{
			foreach (var element in Elements)
			{
				if (element is IUIWidget)
				{
					(element as IUIWidget).PerformLayout();
				}
			}
		}

		public virtual void OnMouseMove(Point mousePos)
		{
			foreach (var element in Elements)
            {
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseMove(mousePos);
				}
            }
		}

		public virtual void OnMouseDown(Point mousePos, MouseButton button)
		{
			foreach (var element in Elements)
			{
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseDown(mousePos, button);
				}
			}
		}

		public virtual void OnMouseUp(Point mousePos, MouseButton button)
		{
			foreach (var element in Elements)
			{
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseUp(mousePos, button);
				}
			}
		}

        public override void Destroy()
        {
			foreach (var element in Elements)
			{
				element.Destroy();
			}
            base.Destroy();
        }
    }
}

