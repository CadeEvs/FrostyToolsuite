
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolRecordPropertyTrackData))]
	public class BoolRecordPropertyTrack : RecordPropertyTrackBase, IEntityData<FrostySdk.Ebx.BoolRecordPropertyTrackData>
	{
		public new FrostySdk.Ebx.BoolRecordPropertyTrackData Data => data as FrostySdk.Ebx.BoolRecordPropertyTrackData;
		public override string DisplayName => "BoolRecordPropertyTrack";

		public BoolRecordPropertyTrack(FrostySdk.Ebx.BoolRecordPropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

