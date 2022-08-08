using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIWeaponRuleEntityData))]
	public class SquadronAIWeaponRuleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIWeaponRuleEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIWeaponRuleEntityData Data => data as FrostySdk.Ebx.SquadronAIWeaponRuleEntityData;
		public override string DisplayName => "SquadronAIWeaponRule";

		public SquadronAIWeaponRuleEntity(FrostySdk.Ebx.SquadronAIWeaponRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

