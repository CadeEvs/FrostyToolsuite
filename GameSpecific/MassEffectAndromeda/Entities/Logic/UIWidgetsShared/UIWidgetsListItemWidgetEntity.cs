using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsListItemWidgetEntityData))]
	public class UIWidgetsListItemWidgetEntity : UIWidgetsContainerItemWidgetEntity, IEntityData<FrostySdk.Ebx.UIWidgetsListItemWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsListItemWidgetEntityData Data => data as FrostySdk.Ebx.UIWidgetsListItemWidgetEntityData;
		public override string DisplayName => "UIWidgetsListItemWidget";

		public UIWidgetsListItemWidgetEntity(FrostySdk.Ebx.UIWidgetsListItemWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

