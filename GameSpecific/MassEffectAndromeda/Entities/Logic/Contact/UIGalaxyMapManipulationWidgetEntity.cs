using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIGalaxyMapManipulationWidgetEntityData))]
	public class UIGalaxyMapManipulationWidgetEntity : UIMapManipulationWidgetBase, IEntityData<FrostySdk.Ebx.UIGalaxyMapManipulationWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIGalaxyMapManipulationWidgetEntityData Data => data as FrostySdk.Ebx.UIGalaxyMapManipulationWidgetEntityData;
		public override string DisplayName => "UIGalaxyMapManipulationWidget";

		public UIGalaxyMapManipulationWidgetEntity(FrostySdk.Ebx.UIGalaxyMapManipulationWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

