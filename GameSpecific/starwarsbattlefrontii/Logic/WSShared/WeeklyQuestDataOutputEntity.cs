using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeeklyQuestDataOutputEntityData))]
	public class WeeklyQuestDataOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeeklyQuestDataOutputEntityData>
	{
		public new FrostySdk.Ebx.WeeklyQuestDataOutputEntityData Data => data as FrostySdk.Ebx.WeeklyQuestDataOutputEntityData;
		public override string DisplayName => "WeeklyQuestDataOutput";

		public WeeklyQuestDataOutputEntity(FrostySdk.Ebx.WeeklyQuestDataOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

