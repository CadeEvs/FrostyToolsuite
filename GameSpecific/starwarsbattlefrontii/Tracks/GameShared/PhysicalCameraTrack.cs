
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicalCameraTrackData))]
	public class PhysicalCameraTrack : CameraTrackBase, IEntityData<FrostySdk.Ebx.PhysicalCameraTrackData>
	{
		public new FrostySdk.Ebx.PhysicalCameraTrackData Data => data as FrostySdk.Ebx.PhysicalCameraTrackData;
		public override string DisplayName => "PhysicalCameraTrack";

		public PhysicalCameraTrack(FrostySdk.Ebx.PhysicalCameraTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

