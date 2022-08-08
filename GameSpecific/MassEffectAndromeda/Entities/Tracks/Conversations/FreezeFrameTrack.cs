
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FreezeFrameTrackData))]
	public class FreezeFrameTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.FreezeFrameTrackData>
	{
		public new FrostySdk.Ebx.FreezeFrameTrackData Data => data as FrostySdk.Ebx.FreezeFrameTrackData;
		public override string DisplayName => "FreezeFrameTrack";

		public FreezeFrameTrack(FrostySdk.Ebx.FreezeFrameTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

