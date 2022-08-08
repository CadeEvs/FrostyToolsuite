
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraFocusTrackData))]
	public class CameraFocusTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.CameraFocusTrackData>
	{
		public new FrostySdk.Ebx.CameraFocusTrackData Data => data as FrostySdk.Ebx.CameraFocusTrackData;
		public override string DisplayName => "CameraFocusTrack";

		public CameraFocusTrack(FrostySdk.Ebx.CameraFocusTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

