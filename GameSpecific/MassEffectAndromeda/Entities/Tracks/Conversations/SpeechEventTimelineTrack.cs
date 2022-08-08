
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpeechEventTimelineTrackData))]
	public class SpeechEventTimelineTrack : LinkTrack, IEntityData<FrostySdk.Ebx.SpeechEventTimelineTrackData>
	{
		public new FrostySdk.Ebx.SpeechEventTimelineTrackData Data => data as FrostySdk.Ebx.SpeechEventTimelineTrackData;
		public override string DisplayName => "SpeechEventTimelineTrack";

		public SpeechEventTimelineTrack(FrostySdk.Ebx.SpeechEventTimelineTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

