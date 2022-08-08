using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DeliverRewardEntityData))]
	public class DeliverRewardEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DeliverRewardEntityData>
	{
		public new FrostySdk.Ebx.DeliverRewardEntityData Data => data as FrostySdk.Ebx.DeliverRewardEntityData;
		public override string DisplayName => "DeliverReward";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DeliverRewardEntity(FrostySdk.Ebx.DeliverRewardEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

