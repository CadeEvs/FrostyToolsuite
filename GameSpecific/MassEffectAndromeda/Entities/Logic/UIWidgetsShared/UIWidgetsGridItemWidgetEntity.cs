using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsGridItemWidgetEntityData))]
	public class UIWidgetsGridItemWidgetEntity : UIWidgetsContainerItemWidgetEntity, IEntityData<FrostySdk.Ebx.UIWidgetsGridItemWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsGridItemWidgetEntityData Data => data as FrostySdk.Ebx.UIWidgetsGridItemWidgetEntityData;
		public override string DisplayName => "UIWidgetsGridItemWidget";

		public UIWidgetsGridItemWidgetEntity(FrostySdk.Ebx.UIWidgetsGridItemWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

