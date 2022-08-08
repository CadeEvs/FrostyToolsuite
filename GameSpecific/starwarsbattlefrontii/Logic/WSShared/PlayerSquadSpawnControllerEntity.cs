using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerSquadSpawnControllerEntityData))]
	public class PlayerSquadSpawnControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerSquadSpawnControllerEntityData>
	{
		public new FrostySdk.Ebx.PlayerSquadSpawnControllerEntityData Data => data as FrostySdk.Ebx.PlayerSquadSpawnControllerEntityData;
		public override string DisplayName => "PlayerSquadSpawnController";

		public PlayerSquadSpawnControllerEntity(FrostySdk.Ebx.PlayerSquadSpawnControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

