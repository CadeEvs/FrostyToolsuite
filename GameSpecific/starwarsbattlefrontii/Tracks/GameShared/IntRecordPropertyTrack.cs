
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntRecordPropertyTrackData))]
	public class IntRecordPropertyTrack : RecordPropertyTrackBase, IEntityData<FrostySdk.Ebx.IntRecordPropertyTrackData>
	{
		public new FrostySdk.Ebx.IntRecordPropertyTrackData Data => data as FrostySdk.Ebx.IntRecordPropertyTrackData;
		public override string DisplayName => "IntRecordPropertyTrack";

		public IntRecordPropertyTrack(FrostySdk.Ebx.IntRecordPropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

