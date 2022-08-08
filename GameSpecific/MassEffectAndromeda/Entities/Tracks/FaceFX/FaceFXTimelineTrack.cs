
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FaceFXTimelineTrackData))]
	public class FaceFXTimelineTrack : LinkTrack, IEntityData<FrostySdk.Ebx.FaceFXTimelineTrackData>
	{
		public new FrostySdk.Ebx.FaceFXTimelineTrackData Data => data as FrostySdk.Ebx.FaceFXTimelineTrackData;
		public override string DisplayName => "FaceFXTimelineTrack";

		public FaceFXTimelineTrack(FrostySdk.Ebx.FaceFXTimelineTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

