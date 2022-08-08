using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsContainerWidgetEntityData))]
	public class UIWidgetsContainerWidgetEntity : UIWidgetsWidgetEntity, IEntityData<FrostySdk.Ebx.UIWidgetsContainerWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsContainerWidgetEntityData Data => data as FrostySdk.Ebx.UIWidgetsContainerWidgetEntityData;
		public override string DisplayName => "UIWidgetsContainerWidget";

		public UIWidgetsContainerWidgetEntity(FrostySdk.Ebx.UIWidgetsContainerWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

