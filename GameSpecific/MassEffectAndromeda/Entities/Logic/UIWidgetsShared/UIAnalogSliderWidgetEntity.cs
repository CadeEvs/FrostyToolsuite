using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIAnalogSliderWidgetEntityData))]
	public class UIAnalogSliderWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIAnalogSliderWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIAnalogSliderWidgetEntityData Data => data as FrostySdk.Ebx.UIAnalogSliderWidgetEntityData;
		public override string DisplayName => "UIAnalogSliderWidget";

		public UIAnalogSliderWidgetEntity(FrostySdk.Ebx.UIAnalogSliderWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

