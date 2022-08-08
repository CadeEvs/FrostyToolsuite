using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MatchEventsData))]
	public class MatchEvents : LogicEntity, IEntityData<FrostySdk.Ebx.MatchEventsData>
	{
		public new FrostySdk.Ebx.MatchEventsData Data => data as FrostySdk.Ebx.MatchEventsData;
		public override string DisplayName => "MatchEvents";

		public MatchEvents(FrostySdk.Ebx.MatchEventsData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

