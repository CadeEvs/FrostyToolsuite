using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientRankRewardEntityData))]
	public class ClientRankRewardEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientRankRewardEntityData>
	{
		public new FrostySdk.Ebx.ClientRankRewardEntityData Data => data as FrostySdk.Ebx.ClientRankRewardEntityData;
		public override string DisplayName => "ClientRankReward";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientRankRewardEntity(FrostySdk.Ebx.ClientRankRewardEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

