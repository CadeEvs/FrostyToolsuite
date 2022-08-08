using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ButtonLegendLayout = FrostySdk.Ebx.ButtonLegendLayout;
using UIElementWidgetReferenceEntityData = FrostySdk.Ebx.UIElementWidgetReferenceEntityData;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIButtonLegendWidgetEntityData))]
	public class UIButtonLegendWidgetEntity : UIChildItemSpawnerWidgetEntity, IEntityData<FrostySdk.Ebx.UIButtonLegendWidgetEntityData>
	{
		protected readonly int Property_Layout = Frosty.Hash.Fnv1.HashString("Layout");

		public new FrostySdk.Ebx.UIButtonLegendWidgetEntityData Data => data as FrostySdk.Ebx.UIButtonLegendWidgetEntityData;
		public override string DisplayName => "UIButtonLegendWidget";
		public override IEnumerable<ConnectionDesc> ChildProperties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Layout", Direction.In, typeof(ButtonLegendLayout))
			};
		}

		protected Event<OutputEvent> event_1050b287;
		protected Property<ButtonLegendLayout> layoutProperty;

		public UIButtonLegendWidgetEntity(FrostySdk.Ebx.UIButtonLegendWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			event_1050b287 = new Event<OutputEvent>(this, 0x1050b287);
			layoutProperty = new Property<ButtonLegendLayout>(this, Property_Layout);

			var parentRefEntity = FindAncestor<UIElementWidgetReferenceEntity>();
			parentRefEntity.AddProperty(layoutProperty);
		}

		public override void Update_PreFrame()
		{
			base.Update_PreFrame();
			foreach (var entity in childEntities)
			{
				if (entity is ISchematicsType)
				{
					var schematicsEntity = entity as ISchematicsType;
					schematicsEntity.Update_PreFrame();
				}
			}
		}

		public override void Update_PostFrame()
		{
			base.Update_PostFrame();
			foreach (var entity in childEntities)
			{
				if (entity is ISchematicsType)
				{
					var schematicsEntity = entity as ISchematicsType;
					schematicsEntity.Update_PostFrame();
				}
			}
		}

		public override void OnPropertyChanged(int propertyHash)
		{
			if (propertyHash == layoutProperty.NameHash)
			{
				UpdateLayout();
				return;
			}

			base.OnPropertyChanged(propertyHash);
		}

		public override void EndSimulation()
		{
			base.EndSimulation();
			ClearSimulatedEntities();
		}

		private void UpdateLayout()
		{
			ClearSimulatedEntities();

			var layout = layoutProperty.Value;
			Application.Current.Dispatcher.Invoke(() =>
			{
				float positionX = 0;
				for (int i = 0; i < layout.Buttons.Count; i++)
				{
					var refrData = new UIElementWidgetReferenceEntityData()
					{
						Blueprint = Data.ChildItemBlueprint,
						Alpha = 1.0f,
						Offset = new FrostySdk.Ebx.UIElementOffset()
						{
							X = 0,
							Y = 0
						}
					};

					var childEntity = (UIElementWidgetReferenceEntity)CreateEntity(refrData, Parent);
					//SchematicsSimulationWorld.EntitiesToCreate.Add(childEntity);

					if (layout.Alignment == FrostySdk.Ebx.ButtonLegendAlignment.ButtonLegendAlignment_Center)
					{
						if (positionX == 0)
						{
							var parentRef = parent as UIElementWidgetReferenceEntity;

							positionX = (childEntity.RootEntity.Data.Size.X * layout.Buttons.Count) + (Data.InterButtonSpacing * (layout.Buttons.Count - 1));
							positionX = ((float)parentRef.LayoutSize.Width - positionX) * 0.5f;
						}
					}

					childEntity.RedoLayout(new Point(positionX, 0));
					positionX += childEntity.RootEntity.Data.Size.X + Data.InterButtonSpacing;

					childEntities.Add(childEntity);
				}
			});

			for (int i = 0; i < childEntities.Count; i++)
			{
				var childEntity = childEntities[i] as UIElementWidgetReferenceEntity;
				var data = layout.Buttons[i].GetObjectAs<object>();

				(childEntities[i] as ISchematicsType).BeginSimulation();

				// try to find the child item interface
				var childItemInterface = childEntity.RootEntity.Components.FirstOrDefault(e => e is UIChildItemInterface);
				if (childItemInterface != null)
				{
					// and set the data source
					(childItemInterface as UIChildItemInterface).DataSource = data;
				}

				if (i == 0)
				{
					// select the first one in the list
					var evt = (childEntities[0] as ISchematicsType).GetEvent(-530029867);
					if (evt != null)
					{
						evt.Execute();
					}
				}
			}

			event_1050b287.Execute();
		}

		private void ClearSimulatedEntities()
		{
			//Application.Current.Dispatcher.Invoke(() =>
			//{
			//	foreach (var childEntity in childEntities)
			//	{
			//		(childEntity as ISchematicsType).EndSimulation();
			//		SchematicsSimulationWorld.EntitiesToRemove.Add(childEntity);
			//	}
			//	childEntities.Clear();
			//});
		}
	}
}

