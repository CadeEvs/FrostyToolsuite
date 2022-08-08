using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPWaveData))]
	public class SpawnBundleGroupRequestsEntity_MPWave : SpawnBundleGroupRequestsEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPWaveData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPWaveData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPWaveData;
		public override string DisplayName => "SpawnBundleGroupRequestsEntity_MPWave";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpawnBundleGroupRequestsEntity_MPWave(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPWaveData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

