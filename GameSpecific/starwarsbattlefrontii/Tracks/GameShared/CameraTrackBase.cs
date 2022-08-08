
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraTrackBaseData))]
	public class CameraTrackBase : TimelineTrack, IEntityData<FrostySdk.Ebx.CameraTrackBaseData>
	{
		public new FrostySdk.Ebx.CameraTrackBaseData Data => data as FrostySdk.Ebx.CameraTrackBaseData;
		public override string DisplayName => "CameraTrackBase";

		public CameraTrackBase(FrostySdk.Ebx.CameraTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

