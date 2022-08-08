using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnemySpawnPointData))]
	public class EnemySpawnPoint : EnemySpawnObject, IEntityData<FrostySdk.Ebx.EnemySpawnPointData>
	{
		public new FrostySdk.Ebx.EnemySpawnPointData Data => data as FrostySdk.Ebx.EnemySpawnPointData;

		public EnemySpawnPoint(FrostySdk.Ebx.EnemySpawnPointData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

