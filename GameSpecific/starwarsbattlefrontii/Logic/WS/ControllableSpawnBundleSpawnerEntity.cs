using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData))]
	public class ControllableSpawnBundleSpawnerEntity : SpawnBundleSpawnerEntity, IEntityData<FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData>
	{
		public new FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData Data => data as FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData;
		public override string DisplayName => "ControllableSpawnBundleSpawner";

		public ControllableSpawnBundleSpawnerEntity(FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

