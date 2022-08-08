using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChangeTeamCombatDirectiveEntityData))]
	public class ChangeTeamCombatDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChangeTeamCombatDirectiveEntityData>
	{
		public new FrostySdk.Ebx.ChangeTeamCombatDirectiveEntityData Data => data as FrostySdk.Ebx.ChangeTeamCombatDirectiveEntityData;
		public override string DisplayName => "ChangeTeamCombatDirective";

		public ChangeTeamCombatDirectiveEntity(FrostySdk.Ebx.ChangeTeamCombatDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

