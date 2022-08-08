using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestCharacterCombatDirectiveEntityData))]
	public class TestCharacterCombatDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestCharacterCombatDirectiveEntityData>
	{
		public new FrostySdk.Ebx.TestCharacterCombatDirectiveEntityData Data => data as FrostySdk.Ebx.TestCharacterCombatDirectiveEntityData;
		public override string DisplayName => "TestCharacterCombatDirective";

		public TestCharacterCombatDirectiveEntity(FrostySdk.Ebx.TestCharacterCombatDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

