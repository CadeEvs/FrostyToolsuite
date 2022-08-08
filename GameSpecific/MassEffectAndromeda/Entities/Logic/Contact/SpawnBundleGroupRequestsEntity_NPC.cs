using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_NPCData))]
	public class SpawnBundleGroupRequestsEntity_NPC : SpawnBundleGroupRequestsEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_NPCData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_NPCData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_NPCData;
		public override string DisplayName => "SpawnBundleGroupRequestsEntity_NPC";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpawnBundleGroupRequestsEntity_NPC(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_NPCData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

