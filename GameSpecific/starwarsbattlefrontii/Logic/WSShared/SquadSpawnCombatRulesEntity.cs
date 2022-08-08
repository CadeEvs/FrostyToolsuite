using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadSpawnCombatRulesEntityData))]
	public class SquadSpawnCombatRulesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadSpawnCombatRulesEntityData>
	{
		public new FrostySdk.Ebx.SquadSpawnCombatRulesEntityData Data => data as FrostySdk.Ebx.SquadSpawnCombatRulesEntityData;
		public override string DisplayName => "SquadSpawnCombatRules";

		public SquadSpawnCombatRulesEntity(FrostySdk.Ebx.SquadSpawnCombatRulesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

