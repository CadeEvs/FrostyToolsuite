using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerSpawnControllerEntityData))]
	public class PlayerSpawnControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerSpawnControllerEntityData>
	{
		public new FrostySdk.Ebx.PlayerSpawnControllerEntityData Data => data as FrostySdk.Ebx.PlayerSpawnControllerEntityData;
		public override string DisplayName => "PlayerSpawnController";

		public PlayerSpawnControllerEntity(FrostySdk.Ebx.PlayerSpawnControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

