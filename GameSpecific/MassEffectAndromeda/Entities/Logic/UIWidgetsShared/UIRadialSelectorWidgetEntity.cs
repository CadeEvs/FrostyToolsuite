using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIRadialSelectorWidgetEntityData))]
	public class UIRadialSelectorWidgetEntity : UIBaseSelectorWidgetEntity, IEntityData<FrostySdk.Ebx.UIRadialSelectorWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIRadialSelectorWidgetEntityData Data => data as FrostySdk.Ebx.UIRadialSelectorWidgetEntityData;
		public override string DisplayName => "UIRadialSelectorWidget";

		public UIRadialSelectorWidgetEntity(FrostySdk.Ebx.UIRadialSelectorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

