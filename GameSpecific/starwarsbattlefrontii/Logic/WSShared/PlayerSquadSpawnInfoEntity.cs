using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerSquadSpawnInfoEntityData))]
	public class PlayerSquadSpawnInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerSquadSpawnInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerSquadSpawnInfoEntityData Data => data as FrostySdk.Ebx.PlayerSquadSpawnInfoEntityData;
		public override string DisplayName => "PlayerSquadSpawnInfo";

		public PlayerSquadSpawnInfoEntity(FrostySdk.Ebx.PlayerSquadSpawnInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

