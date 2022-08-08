using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnemyWaveGeneratorData))]
	public class EnemyWaveGenerator : LogicEntity, IEntityData<FrostySdk.Ebx.EnemyWaveGeneratorData>
	{
		public new FrostySdk.Ebx.EnemyWaveGeneratorData Data => data as FrostySdk.Ebx.EnemyWaveGeneratorData;
		public override string DisplayName => "EnemyWaveGenerator";

		public EnemyWaveGenerator(FrostySdk.Ebx.EnemyWaveGeneratorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

