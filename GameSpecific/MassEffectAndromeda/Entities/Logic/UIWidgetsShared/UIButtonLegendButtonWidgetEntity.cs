using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIButtonLegendButtonWidgetEntityData))]
	public class UIButtonLegendButtonWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIButtonLegendButtonWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIButtonLegendButtonWidgetEntityData Data => data as FrostySdk.Ebx.UIButtonLegendButtonWidgetEntityData;
		public override string DisplayName => "UIButtonLegendButtonWidget";

		public UIButtonLegendButtonWidgetEntity(FrostySdk.Ebx.UIButtonLegendButtonWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

