using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerCharacterSpawnEntityData))]
	public class PlayerCharacterSpawnEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerCharacterSpawnEntityData>
	{
		public new FrostySdk.Ebx.PlayerCharacterSpawnEntityData Data => data as FrostySdk.Ebx.PlayerCharacterSpawnEntityData;
		public override string DisplayName => "PlayerCharacterSpawn";

		public PlayerCharacterSpawnEntity(FrostySdk.Ebx.PlayerCharacterSpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

