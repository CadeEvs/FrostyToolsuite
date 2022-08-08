using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingTopPrioRuleEntityData))]
	public class TargetingTopPrioRuleEntity : TargetingRuleEntity, IEntityData<FrostySdk.Ebx.TargetingTopPrioRuleEntityData>
	{
		public new FrostySdk.Ebx.TargetingTopPrioRuleEntityData Data => data as FrostySdk.Ebx.TargetingTopPrioRuleEntityData;
		public override string DisplayName => "TargetingTopPrioRule";

		public TargetingTopPrioRuleEntity(FrostySdk.Ebx.TargetingTopPrioRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

