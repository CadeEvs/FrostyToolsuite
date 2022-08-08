
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformRecordPropertyTrackData))]
	public class TransformRecordPropertyTrack : RecordPropertyTrackBase, IEntityData<FrostySdk.Ebx.TransformRecordPropertyTrackData>
	{
		public new FrostySdk.Ebx.TransformRecordPropertyTrackData Data => data as FrostySdk.Ebx.TransformRecordPropertyTrackData;
		public override string DisplayName => "TransformRecordPropertyTrack";

		public TransformRecordPropertyTrack(FrostySdk.Ebx.TransformRecordPropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

