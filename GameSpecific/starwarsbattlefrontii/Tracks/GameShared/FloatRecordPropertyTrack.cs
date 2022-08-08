
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatRecordPropertyTrackData))]
	public class FloatRecordPropertyTrack : RecordPropertyTrackBase, IEntityData<FrostySdk.Ebx.FloatRecordPropertyTrackData>
	{
		public new FrostySdk.Ebx.FloatRecordPropertyTrackData Data => data as FrostySdk.Ebx.FloatRecordPropertyTrackData;
		public override string DisplayName => "FloatRecordPropertyTrack";

		public FloatRecordPropertyTrack(FrostySdk.Ebx.FloatRecordPropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

