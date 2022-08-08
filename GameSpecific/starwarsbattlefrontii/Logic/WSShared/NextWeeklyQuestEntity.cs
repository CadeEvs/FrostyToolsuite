using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NextWeeklyQuestEntityData))]
	public class NextWeeklyQuestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NextWeeklyQuestEntityData>
	{
		public new FrostySdk.Ebx.NextWeeklyQuestEntityData Data => data as FrostySdk.Ebx.NextWeeklyQuestEntityData;
		public override string DisplayName => "NextWeeklyQuest";

		public NextWeeklyQuestEntity(FrostySdk.Ebx.NextWeeklyQuestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

