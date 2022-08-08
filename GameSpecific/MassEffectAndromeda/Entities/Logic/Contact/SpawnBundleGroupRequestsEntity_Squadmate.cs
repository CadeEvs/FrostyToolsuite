using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_SquadmateData))]
	public class SpawnBundleGroupRequestsEntity_Squadmate : SpawnBundleGroupRequestsEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_SquadmateData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_SquadmateData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_SquadmateData;
		public override string DisplayName => "SpawnBundleGroupRequestsEntity_Squadmate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpawnBundleGroupRequestsEntity_Squadmate(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_SquadmateData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

