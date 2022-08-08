using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChangeCharacterCombatDirectiveEntityData))]
	public class ChangeCharacterCombatDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChangeCharacterCombatDirectiveEntityData>
	{
		public new FrostySdk.Ebx.ChangeCharacterCombatDirectiveEntityData Data => data as FrostySdk.Ebx.ChangeCharacterCombatDirectiveEntityData;
		public override string DisplayName => "ChangeCharacterCombatDirective";

		public ChangeCharacterCombatDirectiveEntity(FrostySdk.Ebx.ChangeCharacterCombatDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

