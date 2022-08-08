using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPProgressionRewardEntityData))]
	public class SPProgressionRewardEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPProgressionRewardEntityData>
	{
		public new FrostySdk.Ebx.SPProgressionRewardEntityData Data => data as FrostySdk.Ebx.SPProgressionRewardEntityData;
		public override string DisplayName => "SPProgressionReward";

		public SPProgressionRewardEntity(FrostySdk.Ebx.SPProgressionRewardEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

