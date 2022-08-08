using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadDamageBonusModifierEntityData))]
	public class SquadDamageBonusModifierEntity : WSHealthComponentDamageModifierEntity, IEntityData<FrostySdk.Ebx.SquadDamageBonusModifierEntityData>
	{
		public new FrostySdk.Ebx.SquadDamageBonusModifierEntityData Data => data as FrostySdk.Ebx.SquadDamageBonusModifierEntityData;
		public override string DisplayName => "SquadDamageBonusModifier";

		public SquadDamageBonusModifierEntity(FrostySdk.Ebx.SquadDamageBonusModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

