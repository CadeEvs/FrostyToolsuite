using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIBreadcrumbChildWidgetData))]
	public class UIBreadcrumbChildWidget : UIWidgetsWidgetEntity, IEntityData<FrostySdk.Ebx.UIBreadcrumbChildWidgetData>
	{
		public new FrostySdk.Ebx.UIBreadcrumbChildWidgetData Data => data as FrostySdk.Ebx.UIBreadcrumbChildWidgetData;
		public override string DisplayName => "UIBreadcrumbChildWidget";

		public UIBreadcrumbChildWidget(FrostySdk.Ebx.UIBreadcrumbChildWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

