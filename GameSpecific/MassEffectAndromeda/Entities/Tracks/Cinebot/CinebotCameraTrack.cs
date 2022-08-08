
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotCameraTrackData))]
	public class CinebotCameraTrack : CameraTrack, IEntityData<FrostySdk.Ebx.CinebotCameraTrackData>
	{
		public new FrostySdk.Ebx.CinebotCameraTrackData Data => data as FrostySdk.Ebx.CinebotCameraTrackData;
		public override string DisplayName => "CinebotCameraTrack";

		public CinebotCameraTrack(FrostySdk.Ebx.CinebotCameraTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.DofTrack);
		}
	}
}

