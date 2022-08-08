using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CampaignDeathExperienceEntityData))]
	public class CampaignDeathExperienceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CampaignDeathExperienceEntityData>
	{
		public new FrostySdk.Ebx.CampaignDeathExperienceEntityData Data => data as FrostySdk.Ebx.CampaignDeathExperienceEntityData;
		public override string DisplayName => "CampaignDeathExperience";

		public CampaignDeathExperienceEntity(FrostySdk.Ebx.CampaignDeathExperienceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

