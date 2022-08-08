using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIHazardSelectorData))]
	public class UIHazardSelector : LogicEntity, IEntityData<FrostySdk.Ebx.UIHazardSelectorData>
	{
		public new FrostySdk.Ebx.UIHazardSelectorData Data => data as FrostySdk.Ebx.UIHazardSelectorData;
		public override string DisplayName => "UIHazardSelector";

		public UIHazardSelector(FrostySdk.Ebx.UIHazardSelectorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

