using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIMissileWeaponRuleEntityData))]
	public class SquadronAIMissileWeaponRuleEntity : SquadronAIWeaponRuleEntity, IEntityData<FrostySdk.Ebx.SquadronAIMissileWeaponRuleEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIMissileWeaponRuleEntityData Data => data as FrostySdk.Ebx.SquadronAIMissileWeaponRuleEntityData;
		public override string DisplayName => "SquadronAIMissileWeaponRule";

		public SquadronAIMissileWeaponRuleEntity(FrostySdk.Ebx.SquadronAIMissileWeaponRuleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

