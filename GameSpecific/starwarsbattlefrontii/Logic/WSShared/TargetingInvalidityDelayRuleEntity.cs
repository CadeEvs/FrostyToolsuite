using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingInvalidityDelayRuleEntityData))]
	public class TargetingInvalidityDelayRuleEntity : TargetingRuleEntity, IEntityData<FrostySdk.Ebx.TargetingInvalidityDelayRuleEntityData>
	{
		public new FrostySdk.Ebx.TargetingInvalidityDelayRuleEntityData Data => data as FrostySdk.Ebx.TargetingInvalidityDelayRuleEntityData;
		public override string DisplayName => "TargetingInvalidityDelayRule";

		public TargetingInvalidityDelayRuleEntity(FrostySdk.Ebx.TargetingInvalidityDelayRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

