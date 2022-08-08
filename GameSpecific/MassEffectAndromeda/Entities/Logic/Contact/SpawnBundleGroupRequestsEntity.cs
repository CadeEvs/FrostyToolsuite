using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntityData))]
	public class SpawnBundleGroupRequestsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntityData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntityData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntityData;
		public override string DisplayName => "SpawnBundleGroupRequests";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Spawners", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnSpawn", Direction.Out)
			};
		}

		public SpawnBundleGroupRequestsEntity(FrostySdk.Ebx.SpawnBundleGroupRequestsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

