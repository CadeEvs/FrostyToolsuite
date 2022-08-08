using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPUIKitData))]
	public class SpawnBundleGroupRequestsEntity_MPUIKit : SpawnBundleGroupRequestsEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPUIKitData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPUIKitData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPUIKitData;
		public override string DisplayName => "SpawnBundleGroupRequestsEntity_MPUIKit";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpawnBundleGroupRequestsEntity_MPUIKit(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPUIKitData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

