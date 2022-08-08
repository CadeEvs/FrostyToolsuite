
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTBlendKeyframeTrackData))]
	public class ANTBlendKeyframeTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.ANTBlendKeyframeTrackData>
	{
		public new FrostySdk.Ebx.ANTBlendKeyframeTrackData Data => data as FrostySdk.Ebx.ANTBlendKeyframeTrackData;
		public override string DisplayName => "ANTBlendKeyframeTrack";

		public ANTBlendKeyframeTrack(FrostySdk.Ebx.ANTBlendKeyframeTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

