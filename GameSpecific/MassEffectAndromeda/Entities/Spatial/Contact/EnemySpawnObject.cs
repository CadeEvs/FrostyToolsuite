using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnemySpawnObjectData))]
	public class EnemySpawnObject : SpatialEntity, IEntityData<FrostySdk.Ebx.EnemySpawnObjectData>
	{
		public new FrostySdk.Ebx.EnemySpawnObjectData Data => data as FrostySdk.Ebx.EnemySpawnObjectData;

		public EnemySpawnObject(FrostySdk.Ebx.EnemySpawnObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

