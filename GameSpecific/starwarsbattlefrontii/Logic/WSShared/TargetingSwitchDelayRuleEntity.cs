using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingSwitchDelayRuleEntityData))]
	public class TargetingSwitchDelayRuleEntity : TargetingRuleEntity, IEntityData<FrostySdk.Ebx.TargetingSwitchDelayRuleEntityData>
	{
		public new FrostySdk.Ebx.TargetingSwitchDelayRuleEntityData Data => data as FrostySdk.Ebx.TargetingSwitchDelayRuleEntityData;
		public override string DisplayName => "TargetingSwitchDelayRule";

		public TargetingSwitchDelayRuleEntity(FrostySdk.Ebx.TargetingSwitchDelayRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

