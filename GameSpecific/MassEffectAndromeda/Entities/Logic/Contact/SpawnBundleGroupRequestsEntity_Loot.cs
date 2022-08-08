using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_LootData))]
	public class SpawnBundleGroupRequestsEntity_Loot : SpawnBundleGroupRequestsEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_LootData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_LootData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_LootData;
		public override string DisplayName => "SpawnBundleGroupRequestsEntity_Loot";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpawnBundleGroupRequestsEntity_Loot(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_LootData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

