using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData))]
	public class ControllableSpawnBundleSpawnerEntity : SpawnBundleSpawnerEntity, IEntityData<FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData>
	{
		public new FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData Data => data as FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData;
		public override string DisplayName => "ControllableSpawnBundleSpawner";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("GroupRequests", Direction.In)
			};
		}

		public ControllableSpawnBundleSpawnerEntity(FrostySdk.Ebx.ControllableSpawnBundleSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

