
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWTwoMovieTrackData))]
	public class BWTwoMovieTrack : GuideTrack, IEntityData<FrostySdk.Ebx.BWTwoMovieTrackData>
	{
		public new FrostySdk.Ebx.BWTwoMovieTrackData Data => data as FrostySdk.Ebx.BWTwoMovieTrackData;
		public override string DisplayName => "BWTwoMovieTrack";

		public BWTwoMovieTrack(FrostySdk.Ebx.BWTwoMovieTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

