
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTClipKeyframeTrackData))]
	public class ANTClipKeyframeTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.ANTClipKeyframeTrackData>
	{
		public new FrostySdk.Ebx.ANTClipKeyframeTrackData Data => data as FrostySdk.Ebx.ANTClipKeyframeTrackData;
		public override string DisplayName => "ANTClipKeyframeTrack";

		public ANTClipKeyframeTrack(FrostySdk.Ebx.ANTClipKeyframeTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

