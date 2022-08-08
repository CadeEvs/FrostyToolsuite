
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraDirectorTrackData))]
	public class CameraDirectorTrack : CameraDirectorTrackBase, IEntityData<FrostySdk.Ebx.CameraDirectorTrackData>
	{
		public new FrostySdk.Ebx.CameraDirectorTrackData Data => data as FrostySdk.Ebx.CameraDirectorTrackData;
		public override string DisplayName => "CameraDirectorTrack";

		public CameraDirectorTrack(FrostySdk.Ebx.CameraDirectorTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

