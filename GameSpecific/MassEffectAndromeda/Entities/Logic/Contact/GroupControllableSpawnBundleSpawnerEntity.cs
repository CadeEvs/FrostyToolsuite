using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroupControllableSpawnBundleSpawnerEntityData))]
	public class GroupControllableSpawnBundleSpawnerEntity : ControllableSpawnBundleSpawnerEntity, IEntityData<FrostySdk.Ebx.GroupControllableSpawnBundleSpawnerEntityData>
	{
		public new FrostySdk.Ebx.GroupControllableSpawnBundleSpawnerEntityData Data => data as FrostySdk.Ebx.GroupControllableSpawnBundleSpawnerEntityData;
		public override string DisplayName => "GroupControllableSpawnBundleSpawner";

		public GroupControllableSpawnBundleSpawnerEntity(FrostySdk.Ebx.GroupControllableSpawnBundleSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

