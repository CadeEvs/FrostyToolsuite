using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingRuleEntityData))]
	public class TargetingRuleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TargetingRuleEntityData>
	{
		public new FrostySdk.Ebx.TargetingRuleEntityData Data => data as FrostySdk.Ebx.TargetingRuleEntityData;
		public override string DisplayName => "TargetingRule";

		public TargetingRuleEntity(FrostySdk.Ebx.TargetingRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

