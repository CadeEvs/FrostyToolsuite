
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraDirectorTrackBaseData))]
	public class CameraDirectorTrackBase : TimelineTrack, IEntityData<FrostySdk.Ebx.CameraDirectorTrackBaseData>
	{
		public new FrostySdk.Ebx.CameraDirectorTrackBaseData Data => data as FrostySdk.Ebx.CameraDirectorTrackBaseData;
		public override string DisplayName => "CameraDirectorTrackBase";

		public CameraDirectorTrackBase(FrostySdk.Ebx.CameraDirectorTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

