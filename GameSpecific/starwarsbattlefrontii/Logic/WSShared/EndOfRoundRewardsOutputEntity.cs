using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EndOfRoundRewardsOutputEntityData))]
	public class EndOfRoundRewardsOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EndOfRoundRewardsOutputEntityData>
	{
		public new FrostySdk.Ebx.EndOfRoundRewardsOutputEntityData Data => data as FrostySdk.Ebx.EndOfRoundRewardsOutputEntityData;
		public override string DisplayName => "EndOfRoundRewardsOutput";

		public EndOfRoundRewardsOutputEntity(FrostySdk.Ebx.EndOfRoundRewardsOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

