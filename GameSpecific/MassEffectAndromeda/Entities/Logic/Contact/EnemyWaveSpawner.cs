using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnemyWaveSpawnerData))]
	public class EnemyWaveSpawner : LogicEntity, IEntityData<FrostySdk.Ebx.EnemyWaveSpawnerData>
	{
		public new FrostySdk.Ebx.EnemyWaveSpawnerData Data => data as FrostySdk.Ebx.EnemyWaveSpawnerData;
		public override string DisplayName => "EnemyWaveSpawner";

		public EnemyWaveSpawner(FrostySdk.Ebx.EnemyWaveSpawnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

