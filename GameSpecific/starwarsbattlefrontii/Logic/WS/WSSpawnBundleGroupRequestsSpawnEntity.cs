using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSpawnBundleGroupRequestsSpawnEntityData))]
	public class WSSpawnBundleGroupRequestsSpawnEntity : SpawnBundleGroupRequestsSpawnEntity, IEntityData<FrostySdk.Ebx.WSSpawnBundleGroupRequestsSpawnEntityData>
	{
		public new FrostySdk.Ebx.WSSpawnBundleGroupRequestsSpawnEntityData Data => data as FrostySdk.Ebx.WSSpawnBundleGroupRequestsSpawnEntityData;
		public override string DisplayName => "WSSpawnBundleGroupRequestsSpawn";

		public WSSpawnBundleGroupRequestsSpawnEntity(FrostySdk.Ebx.WSSpawnBundleGroupRequestsSpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

