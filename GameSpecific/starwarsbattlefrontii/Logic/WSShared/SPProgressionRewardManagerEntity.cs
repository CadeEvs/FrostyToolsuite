using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPProgressionRewardManagerEntityData))]
	public class SPProgressionRewardManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPProgressionRewardManagerEntityData>
	{
		public new FrostySdk.Ebx.SPProgressionRewardManagerEntityData Data => data as FrostySdk.Ebx.SPProgressionRewardManagerEntityData;
		public override string DisplayName => "SPProgressionRewardManager";

		public SPProgressionRewardManagerEntity(FrostySdk.Ebx.SPProgressionRewardManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

