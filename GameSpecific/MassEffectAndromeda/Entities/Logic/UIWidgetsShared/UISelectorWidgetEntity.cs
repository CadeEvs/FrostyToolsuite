using LevelEditorPlugin.Editors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UIElementWidgetReferenceEntityData = FrostySdk.Ebx.UIElementWidgetReferenceEntityData;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISelectorWidgetEntityData))]
	public class UISelectorWidgetEntity : UIBaseSelectorWidgetEntity, IEntityData<FrostySdk.Ebx.UISelectorWidgetEntityData>
	{
		protected readonly int Property_DataProvider = Frosty.Hash.Fnv1.HashString("DataProvider");
		protected readonly int Property_SelectedIndex = Frosty.Hash.Fnv1.HashString("SelectedIndex");
		protected readonly int Property_SelectedData = Frosty.Hash.Fnv1.HashString("SelectedData");

		public new FrostySdk.Ebx.UISelectorWidgetEntityData Data => data as FrostySdk.Ebx.UISelectorWidgetEntityData;
		public override string DisplayName => "UISelectorWidget";
        public override IEnumerable<ConnectionDesc> ChildProperties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("DataProvider", Direction.In),
				new ConnectionDesc("SelectedIndex", Direction.Out, typeof(int)),
				new ConnectionDesc("SelectedData", Direction.Out)
			};
        }

		protected IProperty dataProviderProperty;
		protected IProperty selectedDataProperty;
		protected Property<int> selectedIndexProperty;

        public UISelectorWidgetEntity(FrostySdk.Ebx.UISelectorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			selectedIndexProperty = new Property<int>(this, Property_SelectedIndex);
			dataProviderProperty = new Property<object>(this, Property_DataProvider);
			selectedDataProperty = new Property<object>(this, Property_SelectedData);
			
			var parentRefEntity = FindAncestor<UIElementWidgetReferenceEntity>();

			if (parentRefEntity != null)
			{
				parentRefEntity.AddProperty(dataProviderProperty);
				parentRefEntity.AddProperty(selectedIndexProperty);
				parentRefEntity.AddProperty(selectedDataProperty);
			}
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (dataProviderProperty.IsUnset)
			{
				dataProviderProperty.Value = mockDataList;
			}
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

        public override void EndSimulation()
        {
            base.EndSimulation();
			ClearSimulatedEntities();
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == dataProviderProperty.NameHash)
			{
				UpdateDataProvider();
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }

        private void UpdateDataProvider()
		{
			ClearSimulatedEntities();
			mockDataList = (IList)dataProviderProperty.Value;

			Application.Current.Dispatcher.Invoke(() =>
			{
				float positionX = 0;
				float positionY = 0;

				for (int i = 0; i < mockDataList.Count; i++)
				{
					var refrData = new UIElementWidgetReferenceEntityData()
					{
						Blueprint = Data.ChildItemBlueprint,
						Alpha = 1.0f,
						Offset = new FrostySdk.Ebx.UIElementOffset()
						{
							X = positionX,
							Y = positionY
						}
					};

					var childEntity = (UIElementWidgetReferenceEntity)CreateEntity(refrData, Parent);
					//SchematicsSimulationWorld.EntitiesToCreate.Add(childEntity);

					if (Data.SelectorType == FrostySdk.Ebx.UISelectorType.UISelectorType_Vertical) positionY += childEntity.RootEntity.Data.Size.Y;
					else positionX += childEntity.RootEntity.Data.Size.X;

					childEntities.Add(childEntity);
				}
			});

			for (int i = 0; i < childEntities.Count; i++)
			{
				var childEntity = childEntities[i] as UIElementWidgetReferenceEntity;
				(childEntities[i] as ISchematicsType).BeginSimulation();

				// @todo: handle external refs
				var data = mockDataList[i];
				if (data is FrostySdk.Ebx.PointerRef)
				{
					// redirect pointer refs to the actual object
					data = ((FrostySdk.Ebx.PointerRef)data).GetObjectAs<object>();
				}

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

					selectedIndexProperty.Value = 0;
					selectedDataProperty.Value = data;
				}
			}
		}

		private void ClearSimulatedEntities()
        {
			//Application.Current.Dispatcher.Invoke(() =>
			//{
			//	foreach (var childEntity in childEntities)
			//	{
			//		(childEntity as ISchematicsType).EndSimulation();
			//		childEntity.Destroy();
			//	}
			//	childEntities.Clear();
			//});
		}
    }
}

