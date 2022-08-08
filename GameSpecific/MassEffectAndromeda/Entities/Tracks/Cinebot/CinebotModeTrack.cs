
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotModeTrackData))]
	public class CinebotModeTrack : CameraTrackBase, IEntityData<FrostySdk.Ebx.CinebotModeTrackData>
	{
		public new FrostySdk.Ebx.CinebotModeTrackData Data => data as FrostySdk.Ebx.CinebotModeTrackData;
		public override string DisplayName => "CinebotModeTrack";

		public CinebotModeTrack(FrostySdk.Ebx.CinebotModeTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

