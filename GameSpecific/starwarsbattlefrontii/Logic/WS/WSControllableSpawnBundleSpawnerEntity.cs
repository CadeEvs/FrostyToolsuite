using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSControllableSpawnBundleSpawnerEntityData))]
	public class WSControllableSpawnBundleSpawnerEntity : ControllableSpawnBundleSpawnerEntity, IEntityData<FrostySdk.Ebx.WSControllableSpawnBundleSpawnerEntityData>
	{
		public new FrostySdk.Ebx.WSControllableSpawnBundleSpawnerEntityData Data => data as FrostySdk.Ebx.WSControllableSpawnBundleSpawnerEntityData;
		public override string DisplayName => "WSControllableSpawnBundleSpawner";

		public WSControllableSpawnBundleSpawnerEntity(FrostySdk.Ebx.WSControllableSpawnBundleSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

