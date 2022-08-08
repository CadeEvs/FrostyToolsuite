using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIGridSelectorWidgetEntityData))]
	public class UIGridSelectorWidgetEntity : UIBaseSelectorWidgetEntity, IEntityData<FrostySdk.Ebx.UIGridSelectorWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIGridSelectorWidgetEntityData Data => data as FrostySdk.Ebx.UIGridSelectorWidgetEntityData;
		public override string DisplayName => "UIGridSelectorWidget";

		public UIGridSelectorWidgetEntity(FrostySdk.Ebx.UIGridSelectorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

