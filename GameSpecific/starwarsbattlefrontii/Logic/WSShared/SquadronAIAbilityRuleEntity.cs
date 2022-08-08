using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIAbilityRuleEntityData))]
	public class SquadronAIAbilityRuleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIAbilityRuleEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIAbilityRuleEntityData Data => data as FrostySdk.Ebx.SquadronAIAbilityRuleEntityData;
		public override string DisplayName => "SquadronAIAbilityRule";

		public SquadronAIAbilityRuleEntity(FrostySdk.Ebx.SquadronAIAbilityRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

