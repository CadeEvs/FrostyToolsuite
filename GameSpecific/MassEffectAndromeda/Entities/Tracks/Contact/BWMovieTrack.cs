
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWMovieTrackData))]
	public class BWMovieTrack : GuideTrack, IEntityData<FrostySdk.Ebx.BWMovieTrackData>
	{
		public new FrostySdk.Ebx.BWMovieTrackData Data => data as FrostySdk.Ebx.BWMovieTrackData;
		public override string DisplayName => "BWMovieTrack";

		public BWMovieTrack(FrostySdk.Ebx.BWMovieTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

