
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PoseRecordTrackData))]
	public class PoseRecordTrack : RecordLinkTrackBase, IEntityData<FrostySdk.Ebx.PoseRecordTrackData>
	{
		public new FrostySdk.Ebx.PoseRecordTrackData Data => data as FrostySdk.Ebx.PoseRecordTrackData;
		public override string DisplayName => "PoseRecordTrack";

		public PoseRecordTrack(FrostySdk.Ebx.PoseRecordTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

