using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIWeaponControlEntityData))]
	public class SquadronAIWeaponControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIWeaponControlEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIWeaponControlEntityData Data => data as FrostySdk.Ebx.SquadronAIWeaponControlEntityData;
		public override string DisplayName => "SquadronAIWeaponControl";

		public SquadronAIWeaponControlEntity(FrostySdk.Ebx.SquadronAIWeaponControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

