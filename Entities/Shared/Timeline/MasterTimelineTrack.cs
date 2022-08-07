
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MasterTimelineTrackData))]
	public class MasterTimelineTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.MasterTimelineTrackData>
	{
		public new FrostySdk.Ebx.MasterTimelineTrackData Data => data as FrostySdk.Ebx.MasterTimelineTrackData;
		public override string DisplayName => "MasterTimelineTrack";

		public MasterTimelineTrack(FrostySdk.Ebx.MasterTimelineTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

