
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordTrackData))]
	public class RecordTrack : GroupTrack, IEntityData<FrostySdk.Ebx.RecordTrackData>
	{
		public new FrostySdk.Ebx.RecordTrackData Data => data as FrostySdk.Ebx.RecordTrackData;
		public override string DisplayName => "RecordTrack";

		public RecordTrack(FrostySdk.Ebx.RecordTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

