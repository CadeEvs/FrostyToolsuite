using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPKitData))]
	public class SpawnBundleGroupRequestsEntity_MPKit : SpawnBundleGroupRequestsEntity, IEntityData<FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPKitData>
	{
		public new FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPKitData Data => data as FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPKitData;
		public override string DisplayName => "SpawnBundleGroupRequestsEntity_MPKit";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("PlayerSlot", Direction.In));
				outProperties.Add(new ConnectionDesc("IsPlayerEntered", Direction.In));
				return outProperties;
			}
		}

		public SpawnBundleGroupRequestsEntity_MPKit(FrostySdk.Ebx.SpawnBundleGroupRequestsEntity_MPKitData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

