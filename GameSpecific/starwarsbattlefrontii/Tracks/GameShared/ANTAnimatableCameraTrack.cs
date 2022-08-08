
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTAnimatableCameraTrackData))]
	public class ANTAnimatableCameraTrack : PhysicalCameraTrack, IEntityData<FrostySdk.Ebx.ANTAnimatableCameraTrackData>
	{
		public new FrostySdk.Ebx.ANTAnimatableCameraTrackData Data => data as FrostySdk.Ebx.ANTAnimatableCameraTrackData;
		public override string DisplayName => "ANTAnimatableCameraTrack";

		public ANTAnimatableCameraTrack(FrostySdk.Ebx.ANTAnimatableCameraTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

