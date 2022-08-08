using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeeklyQuestQueryDataOutputEntityData))]
	public class WeeklyQuestQueryDataOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeeklyQuestQueryDataOutputEntityData>
	{
		public new FrostySdk.Ebx.WeeklyQuestQueryDataOutputEntityData Data => data as FrostySdk.Ebx.WeeklyQuestQueryDataOutputEntityData;
		public override string DisplayName => "WeeklyQuestQueryDataOutput";

		public WeeklyQuestQueryDataOutputEntity(FrostySdk.Ebx.WeeklyQuestQueryDataOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

