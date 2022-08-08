using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIDogfighterInfoEntityData))]
	public class SquadronAIDogfighterInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIDogfighterInfoEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIDogfighterInfoEntityData Data => data as FrostySdk.Ebx.SquadronAIDogfighterInfoEntityData;
		public override string DisplayName => "SquadronAIDogfighterInfo";

		public SquadronAIDogfighterInfoEntity(FrostySdk.Ebx.SquadronAIDogfighterInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

