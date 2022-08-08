
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraDirectorProxyTrackData))]
	public class CameraDirectorProxyTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.CameraDirectorProxyTrackData>
	{
		public new FrostySdk.Ebx.CameraDirectorProxyTrackData Data => data as FrostySdk.Ebx.CameraDirectorProxyTrackData;
		public override string DisplayName => "CameraDirectorProxyTrack";

		public CameraDirectorProxyTrack(FrostySdk.Ebx.CameraDirectorProxyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

