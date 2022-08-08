using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetCurrentTrackedQuestData))]
	public class GetCurrentTrackedQuest : LogicEntity, IEntityData<FrostySdk.Ebx.GetCurrentTrackedQuestData>
	{
		public new FrostySdk.Ebx.GetCurrentTrackedQuestData Data => data as FrostySdk.Ebx.GetCurrentTrackedQuestData;
		public override string DisplayName => "GetCurrentTrackedQuest";

		public GetCurrentTrackedQuest(FrostySdk.Ebx.GetCurrentTrackedQuestData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

