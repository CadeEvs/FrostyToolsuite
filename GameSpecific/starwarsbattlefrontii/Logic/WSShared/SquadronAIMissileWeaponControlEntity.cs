using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIMissileWeaponControlEntityData))]
	public class SquadronAIMissileWeaponControlEntity : SquadronAIWeaponControlEntity, IEntityData<FrostySdk.Ebx.SquadronAIMissileWeaponControlEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIMissileWeaponControlEntityData Data => data as FrostySdk.Ebx.SquadronAIMissileWeaponControlEntityData;
		public override string DisplayName => "SquadronAIMissileWeaponControl";

		public SquadronAIMissileWeaponControlEntity(FrostySdk.Ebx.SquadronAIMissileWeaponControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

