using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GhostedExecutingTimelineData))]
	public class GhostedExecutingTimeline : BWExecutingTimeline, IEntityData<FrostySdk.Ebx.GhostedExecutingTimelineData>
	{
		public new FrostySdk.Ebx.GhostedExecutingTimelineData Data => data as FrostySdk.Ebx.GhostedExecutingTimelineData;
		public override string DisplayName => "GhostedExecutingTimeline";

		public GhostedExecutingTimeline(FrostySdk.Ebx.GhostedExecutingTimelineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

