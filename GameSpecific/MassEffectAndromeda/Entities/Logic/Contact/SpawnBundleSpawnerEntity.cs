using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleSpawnerEntityData))]
	public class SpawnBundleSpawnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnBundleSpawnerEntityData>
	{
		public new FrostySdk.Ebx.SpawnBundleSpawnerEntityData Data => data as FrostySdk.Ebx.SpawnBundleSpawnerEntityData;
		public override string DisplayName => "SpawnBundleSpawner";

		public SpawnBundleSpawnerEntity(FrostySdk.Ebx.SpawnBundleSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

