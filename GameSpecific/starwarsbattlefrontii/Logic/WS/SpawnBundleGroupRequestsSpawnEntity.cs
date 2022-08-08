using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsSpawnEntityData))]
	public class SpawnBundleGroupRequestsSpawnEntity : SpawnBundleGroupRequestsStreamingBaseEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsSpawnEntityData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsSpawnEntityData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsSpawnEntityData;
		public override string DisplayName => "SpawnBundleGroupRequestsSpawn";

		public SpawnBundleGroupRequestsSpawnEntity(FrostySdk.Ebx.SpawnBundleGroupRequestsSpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

