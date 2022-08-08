
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameplayCameraTrackData))]
	public class GameplayCameraTrack : CameraTrackBase, IEntityData<FrostySdk.Ebx.GameplayCameraTrackData>
	{
		public new FrostySdk.Ebx.GameplayCameraTrackData Data => data as FrostySdk.Ebx.GameplayCameraTrackData;
		public override string DisplayName => "GameplayCameraTrack";

		public GameplayCameraTrack(FrostySdk.Ebx.GameplayCameraTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

