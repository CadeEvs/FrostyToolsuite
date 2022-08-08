
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioGuideTrackData))]
	public class AudioGuideTrack : GuideTrack, IEntityData<FrostySdk.Ebx.AudioGuideTrackData>
	{
		public new FrostySdk.Ebx.AudioGuideTrackData Data => data as FrostySdk.Ebx.AudioGuideTrackData;
		public override string DisplayName => "AudioGuideTrack";

		public AudioGuideTrack(FrostySdk.Ebx.AudioGuideTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

