
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotIntTrackData))]
	public class CinebotIntTrack : IntTrack, IEntityData<FrostySdk.Ebx.CinebotIntTrackData>
	{
		public new FrostySdk.Ebx.CinebotIntTrackData Data => data as FrostySdk.Ebx.CinebotIntTrackData;
		public override string DisplayName => "CinebotIntTrack";

		public CinebotIntTrack(FrostySdk.Ebx.CinebotIntTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

