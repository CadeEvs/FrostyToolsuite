
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTBoolTrackData))]
	public class ANTBoolTrack : BoolTrack, IEntityData<FrostySdk.Ebx.ANTBoolTrackData>
	{
		public new FrostySdk.Ebx.ANTBoolTrackData Data => data as FrostySdk.Ebx.ANTBoolTrackData;
		public override string DisplayName => "ANTBoolTrack";

		public ANTBoolTrack(FrostySdk.Ebx.ANTBoolTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

