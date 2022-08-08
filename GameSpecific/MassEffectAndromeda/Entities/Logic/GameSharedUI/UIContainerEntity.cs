using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIContainerEntityData))]
	public class UIContainerEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.UIContainerEntityData>, IContainerOfEntities
	{
		public new FrostySdk.Ebx.UIContainerEntityData Data => data as FrostySdk.Ebx.UIContainerEntityData;
		public override string DisplayName => "UIContainer";
		public IEnumerable<Entity> Elements => elements;

		protected List<Entity> elements = new List<Entity>();

		public UIContainerEntity(FrostySdk.Ebx.UIContainerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			foreach (var objRef in Data.Elements)
			{
				var objectData = objRef.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
				var element = CreateEntity(objectData);

				if (element != null)
				{
					elements.Add(element);
				}
			}
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

        public override void PerformLayout()
        {
            base.PerformLayout();
			foreach (var element in Elements)
			{
				if (element is IUIWidget)
                {
					(element as IUIWidget).PerformLayout();
                }
			}
        }

        public override void OnMouseMove(Point mousePos)
        {
            foreach (var element in Elements)
            {
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseMove(mousePos);
				}
            }
        }

		public override void OnMouseDown(Point mousePos, MouseButton button)
		{
			foreach (var element in Elements)
			{
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseDown(mousePos, button);
				}
			}
		}

		public override void OnMouseUp(Point mousePos, MouseButton button)
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

