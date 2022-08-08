using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClaimRewardEntityData))]
	public class ClaimRewardEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClaimRewardEntityData>
	{
		public new FrostySdk.Ebx.ClaimRewardEntityData Data => data as FrostySdk.Ebx.ClaimRewardEntityData;
		public override string DisplayName => "ClaimReward";

		public ClaimRewardEntity(FrostySdk.Ebx.ClaimRewardEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

