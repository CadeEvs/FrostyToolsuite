
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StageCameraTrackData))]
	public class StageCameraTrack : CinebotCameraTrack, IEntityData<FrostySdk.Ebx.StageCameraTrackData>
	{
		public new FrostySdk.Ebx.StageCameraTrackData Data => data as FrostySdk.Ebx.StageCameraTrackData;
		public override string DisplayName => "StageCameraTrack";

		public StageCameraTrack(FrostySdk.Ebx.StageCameraTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

