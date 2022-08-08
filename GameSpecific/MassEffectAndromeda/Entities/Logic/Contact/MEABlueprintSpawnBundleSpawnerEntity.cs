using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEABlueprintSpawnBundleSpawnerEntityData))]
	public class MEABlueprintSpawnBundleSpawnerEntity : SpawnBundleSpawnerEntity, IEntityData<FrostySdk.Ebx.MEABlueprintSpawnBundleSpawnerEntityData>
	{
		public new FrostySdk.Ebx.MEABlueprintSpawnBundleSpawnerEntityData Data => data as FrostySdk.Ebx.MEABlueprintSpawnBundleSpawnerEntityData;
		public override string DisplayName => "MEABlueprintSpawnBundleSpawner";

		public MEABlueprintSpawnBundleSpawnerEntity(FrostySdk.Ebx.MEABlueprintSpawnBundleSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

