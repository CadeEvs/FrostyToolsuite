
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinkedMasterTimelineTrackData))]
	public class LinkedMasterTimelineTrack : LinkTrack, IEntityData<FrostySdk.Ebx.LinkedMasterTimelineTrackData>
	{
		public new FrostySdk.Ebx.LinkedMasterTimelineTrackData Data => data as FrostySdk.Ebx.LinkedMasterTimelineTrackData;
		public override string DisplayName => "LinkedMasterTimelineTrack";

		public LinkedMasterTimelineTrack(FrostySdk.Ebx.LinkedMasterTimelineTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

