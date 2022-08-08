using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsWidgetEntityData))]
	public class UIWidgetsWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIWidgetsWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsWidgetEntityData Data => data as FrostySdk.Ebx.UIWidgetsWidgetEntityData;
		public override string DisplayName => "UIWidgetsWidget";

		public UIWidgetsWidgetEntity(FrostySdk.Ebx.UIWidgetsWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

