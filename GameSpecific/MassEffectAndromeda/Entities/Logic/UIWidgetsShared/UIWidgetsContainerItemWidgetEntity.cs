using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsContainerItemWidgetEntityData))]
	public class UIWidgetsContainerItemWidgetEntity : UIWidgetsContainerWidgetEntity, IEntityData<FrostySdk.Ebx.UIWidgetsContainerItemWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsContainerItemWidgetEntityData Data => data as FrostySdk.Ebx.UIWidgetsContainerItemWidgetEntityData;
		public override string DisplayName => "UIWidgetsContainerItemWidget";

		public UIWidgetsContainerItemWidgetEntity(FrostySdk.Ebx.UIWidgetsContainerItemWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

