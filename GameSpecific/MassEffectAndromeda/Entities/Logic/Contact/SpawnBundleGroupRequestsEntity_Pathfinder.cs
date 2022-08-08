using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_PathfinderData))]
	public class SpawnBundleGroupRequestsEntity_Pathfinder : SpawnBundleGroupRequestsEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_PathfinderData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_PathfinderData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_PathfinderData;
		public override string DisplayName => "SpawnBundleGroupRequestsEntity_Pathfinder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpawnBundleGroupRequestsEntity_Pathfinder(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_PathfinderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

