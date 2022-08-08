using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetCampaignDifficultyEntityData))]
	public class SetCampaignDifficultyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetCampaignDifficultyEntityData>
	{
		public new FrostySdk.Ebx.SetCampaignDifficultyEntityData Data => data as FrostySdk.Ebx.SetCampaignDifficultyEntityData;
		public override string DisplayName => "SetCampaignDifficulty";

		public SetCampaignDifficultyEntity(FrostySdk.Ebx.SetCampaignDifficultyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

