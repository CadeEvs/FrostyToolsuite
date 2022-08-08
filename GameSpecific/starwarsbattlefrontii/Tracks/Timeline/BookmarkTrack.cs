
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BookmarkTrackData))]
	public class BookmarkTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.BookmarkTrackData>
	{
		public new FrostySdk.Ebx.BookmarkTrackData Data => data as FrostySdk.Ebx.BookmarkTrackData;
		public override string DisplayName => "BookmarkTrack";

		public BookmarkTrack(FrostySdk.Ebx.BookmarkTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

