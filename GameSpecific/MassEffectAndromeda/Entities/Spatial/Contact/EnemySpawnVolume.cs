using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnemySpawnVolumeData))]
	public class EnemySpawnVolume : EnemySpawnObject, IEntityData<FrostySdk.Ebx.EnemySpawnVolumeData>
	{
		public new FrostySdk.Ebx.EnemySpawnVolumeData Data => data as FrostySdk.Ebx.EnemySpawnVolumeData;

		public EnemySpawnVolume(FrostySdk.Ebx.EnemySpawnVolumeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

