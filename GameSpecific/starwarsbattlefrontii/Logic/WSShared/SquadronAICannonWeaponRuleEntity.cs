using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAICannonWeaponRuleEntityData))]
	public class SquadronAICannonWeaponRuleEntity : SquadronAIWeaponRuleEntity, IEntityData<FrostySdk.Ebx.SquadronAICannonWeaponRuleEntityData>
	{
		public new FrostySdk.Ebx.SquadronAICannonWeaponRuleEntityData Data => data as FrostySdk.Ebx.SquadronAICannonWeaponRuleEntityData;
		public override string DisplayName => "SquadronAICannonWeaponRule";

		public SquadronAICannonWeaponRuleEntity(FrostySdk.Ebx.SquadronAICannonWeaponRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

