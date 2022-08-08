
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MovieTrackData))]
	public class MovieTrack : GuideTrack, IEntityData<FrostySdk.Ebx.MovieTrackData>
	{
		public new FrostySdk.Ebx.MovieTrackData Data => data as FrostySdk.Ebx.MovieTrackData;
		public override string DisplayName => "MovieTrack";

		public MovieTrack(FrostySdk.Ebx.MovieTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

