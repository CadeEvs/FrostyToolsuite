
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraCutVisibilityTrackData))]
	public class CameraCutVisibilityTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.CameraCutVisibilityTrackData>
	{
		public new FrostySdk.Ebx.CameraCutVisibilityTrackData Data => data as FrostySdk.Ebx.CameraCutVisibilityTrackData;
		public override string DisplayName => "CameraCutVisibilityTrack";

		public CameraCutVisibilityTrack(FrostySdk.Ebx.CameraCutVisibilityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

