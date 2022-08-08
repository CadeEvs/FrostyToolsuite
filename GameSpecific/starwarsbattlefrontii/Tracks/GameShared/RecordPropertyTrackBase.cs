
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordPropertyTrackBaseData))]
	public class RecordPropertyTrackBase : PropertyReaderTrackBase, IEntityData<FrostySdk.Ebx.RecordPropertyTrackBaseData>
	{
		public new FrostySdk.Ebx.RecordPropertyTrackBaseData Data => data as FrostySdk.Ebx.RecordPropertyTrackBaseData;
		public override string DisplayName => "RecordPropertyTrackBase";

		public RecordPropertyTrackBase(FrostySdk.Ebx.RecordPropertyTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

