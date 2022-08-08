using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEUIGridSelectorWidgetEntityData))]
	public class MEUIGridSelectorWidgetEntity : UIGridSelectorWidgetEntity, IEntityData<FrostySdk.Ebx.MEUIGridSelectorWidgetEntityData>
	{
		public new FrostySdk.Ebx.MEUIGridSelectorWidgetEntityData Data => data as FrostySdk.Ebx.MEUIGridSelectorWidgetEntityData;
		public override string DisplayName => "MEUIGridSelectorWidget";

		public MEUIGridSelectorWidgetEntity(FrostySdk.Ebx.MEUIGridSelectorWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

