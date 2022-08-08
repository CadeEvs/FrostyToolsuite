using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CampaignMissionFailedEntityData))]
	public class CampaignMissionFailedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CampaignMissionFailedEntityData>
	{
		public new FrostySdk.Ebx.CampaignMissionFailedEntityData Data => data as FrostySdk.Ebx.CampaignMissionFailedEntityData;
		public override string DisplayName => "CampaignMissionFailed";

		public CampaignMissionFailedEntity(FrostySdk.Ebx.CampaignMissionFailedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

