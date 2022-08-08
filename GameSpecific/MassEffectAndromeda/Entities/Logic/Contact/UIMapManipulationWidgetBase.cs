using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIMapManipulationWidgetBaseData))]
	public class UIMapManipulationWidgetBase : UILocationMarkerWidgetEntity, IEntityData<FrostySdk.Ebx.UIMapManipulationWidgetBaseData>
	{
		public new FrostySdk.Ebx.UIMapManipulationWidgetBaseData Data => data as FrostySdk.Ebx.UIMapManipulationWidgetBaseData;
		public override string DisplayName => "UIMapManipulationWidgetBase";

		public UIMapManipulationWidgetBase(FrostySdk.Ebx.UIMapManipulationWidgetBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

