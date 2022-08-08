using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAICannonWeaponControlEntityData))]
	public class SquadronAICannonWeaponControlEntity : SquadronAIWeaponControlEntity, IEntityData<FrostySdk.Ebx.SquadronAICannonWeaponControlEntityData>
	{
		public new FrostySdk.Ebx.SquadronAICannonWeaponControlEntityData Data => data as FrostySdk.Ebx.SquadronAICannonWeaponControlEntityData;
		public override string DisplayName => "SquadronAICannonWeaponControl";

		public SquadronAICannonWeaponControlEntity(FrostySdk.Ebx.SquadronAICannonWeaponControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

