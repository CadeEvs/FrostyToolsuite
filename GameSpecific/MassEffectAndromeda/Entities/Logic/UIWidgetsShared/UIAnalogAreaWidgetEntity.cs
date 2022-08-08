using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIAnalogAreaWidgetEntityData))]
	public class UIAnalogAreaWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIAnalogAreaWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIAnalogAreaWidgetEntityData Data => data as FrostySdk.Ebx.UIAnalogAreaWidgetEntityData;
		public override string DisplayName => "UIAnalogAreaWidget";

		public UIAnalogAreaWidgetEntity(FrostySdk.Ebx.UIAnalogAreaWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

