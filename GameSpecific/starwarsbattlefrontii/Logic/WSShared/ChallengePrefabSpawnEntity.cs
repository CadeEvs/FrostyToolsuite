using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChallengePrefabSpawnEntityData))]
	public class ChallengePrefabSpawnEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChallengePrefabSpawnEntityData>
	{
		public new FrostySdk.Ebx.ChallengePrefabSpawnEntityData Data => data as FrostySdk.Ebx.ChallengePrefabSpawnEntityData;
		public override string DisplayName => "ChallengePrefabSpawn";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ChallengePrefabSpawnEntity(FrostySdk.Ebx.ChallengePrefabSpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

