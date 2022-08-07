
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordTrackBaseData))]
	public class RecordTrackBase : LinkTrack, IEntityData<FrostySdk.Ebx.RecordTrackBaseData>
	{
		public new FrostySdk.Ebx.RecordTrackBaseData Data => data as FrostySdk.Ebx.RecordTrackBaseData;
		public override string DisplayName => "RecordTrackBase";

		public RecordTrackBase(FrostySdk.Ebx.RecordTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

