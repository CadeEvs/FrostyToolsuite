using System;
using System.Collections;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIBaseSelectorWidgetEntityData))]
	public class UIBaseSelectorWidgetEntity : UIChildItemSpawnerWidgetEntity, IEntityData<FrostySdk.Ebx.UIBaseSelectorWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIBaseSelectorWidgetEntityData Data => data as FrostySdk.Ebx.UIBaseSelectorWidgetEntityData;
		public override string DisplayName => "UIBaseSelectorWidget";
		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				outHeaderRows.AddRange(base.HeaderRows);
				if (selectedDataType != null)
				{
					outHeaderRows.Add(selectedDataType.Name);
				}
				return outHeaderRows;
			}
		}

		protected Type selectedDataType;
		protected IList mockDataList;

		public UIBaseSelectorWidgetEntity(FrostySdk.Ebx.UIBaseSelectorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			selectedDataType = FrostySdk.TypeLibrary.GetType(Data.SelectedDataType);
			mockDataObject = new Editors.WrappedListMockData(selectedDataType);
        }

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			mockDataList = (mockDataObject as Editors.WrappedListMockData).ActualList;
        }
    }
}

