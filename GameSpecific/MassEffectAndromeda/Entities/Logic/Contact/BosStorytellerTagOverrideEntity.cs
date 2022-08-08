using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BosStorytellerTagOverrideEntityData))]
	public class BosStorytellerTagOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BosStorytellerTagOverrideEntityData>
	{
		public new FrostySdk.Ebx.BosStorytellerTagOverrideEntityData Data => data as FrostySdk.Ebx.BosStorytellerTagOverrideEntityData;
		public override string DisplayName => "BosStorytellerTagOverride";

		public BosStorytellerTagOverrideEntity(FrostySdk.Ebx.BosStorytellerTagOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

