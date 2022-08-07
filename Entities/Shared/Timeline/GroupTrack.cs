
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroupTrackData))]
	public class GroupTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.GroupTrackData>
	{
		public new FrostySdk.Ebx.GroupTrackData Data => data as FrostySdk.Ebx.GroupTrackData;
		public override string DisplayName => "GroupTrack";

		public GroupTrack(FrostySdk.Ebx.GroupTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

