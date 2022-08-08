
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordEntryInputTrackData))]
	public class RecordEntryInputTrack : RecordTrackChildren, IEntityData<FrostySdk.Ebx.RecordEntryInputTrackData>
	{
		public new FrostySdk.Ebx.RecordEntryInputTrackData Data => data as FrostySdk.Ebx.RecordEntryInputTrackData;
		public override string DisplayName => "RecordEntryInputTrack";

		public RecordEntryInputTrack(FrostySdk.Ebx.RecordEntryInputTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

