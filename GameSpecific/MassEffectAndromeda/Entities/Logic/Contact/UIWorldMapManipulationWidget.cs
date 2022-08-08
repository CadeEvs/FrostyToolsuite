using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWorldMapManipulationWidgetData))]
	public class UIWorldMapManipulationWidget : UIMapManipulationWidgetBase, IEntityData<FrostySdk.Ebx.UIWorldMapManipulationWidgetData>
	{
		public new FrostySdk.Ebx.UIWorldMapManipulationWidgetData Data => data as FrostySdk.Ebx.UIWorldMapManipulationWidgetData;
		public override string DisplayName => "UIWorldMapManipulationWidget";

		public UIWorldMapManipulationWidget(FrostySdk.Ebx.UIWorldMapManipulationWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

