
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraTrackData))]
	public class CameraTrack : PhysicalCameraTrack, IEntityData<FrostySdk.Ebx.CameraTrackData>
	{
		public new FrostySdk.Ebx.CameraTrackData Data => data as FrostySdk.Ebx.CameraTrackData;
		public override string DisplayName => "CameraTrack";

		public CameraTrack(FrostySdk.Ebx.CameraTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.LayeredTransformTrack);
		}
	}
}

