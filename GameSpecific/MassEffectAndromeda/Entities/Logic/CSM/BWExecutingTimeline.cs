using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWExecutingTimelineData))]
	public class BWExecutingTimeline : LogicEntity, IEntityData<FrostySdk.Ebx.BWExecutingTimelineData>
	{
		public new FrostySdk.Ebx.BWExecutingTimelineData Data => data as FrostySdk.Ebx.BWExecutingTimelineData;
		public override string DisplayName => "BWExecutingTimeline";

		public BWExecutingTimeline(FrostySdk.Ebx.BWExecutingTimelineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

