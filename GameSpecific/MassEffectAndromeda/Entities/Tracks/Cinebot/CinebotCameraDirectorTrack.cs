
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotCameraDirectorTrackData))]
	public class CinebotCameraDirectorTrack : CameraDirectorTrackBase, IEntityData<FrostySdk.Ebx.CinebotCameraDirectorTrackData>
	{
		public new FrostySdk.Ebx.CinebotCameraDirectorTrackData Data => data as FrostySdk.Ebx.CinebotCameraDirectorTrackData;
		public override string DisplayName => "CinebotCameraDirectorTrack";

        public CinebotCameraDirectorTrack(FrostySdk.Ebx.CinebotCameraDirectorTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

