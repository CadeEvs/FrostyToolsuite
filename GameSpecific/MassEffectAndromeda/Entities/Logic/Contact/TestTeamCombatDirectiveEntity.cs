using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestTeamCombatDirectiveEntityData))]
	public class TestTeamCombatDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestTeamCombatDirectiveEntityData>
	{
		public new FrostySdk.Ebx.TestTeamCombatDirectiveEntityData Data => data as FrostySdk.Ebx.TestTeamCombatDirectiveEntityData;
		public override string DisplayName => "TestTeamCombatDirective";

		public TestTeamCombatDirectiveEntity(FrostySdk.Ebx.TestTeamCombatDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

