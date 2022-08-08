
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordLinkTrackBaseData))]
	public class RecordLinkTrackBase : LinkTrack, IEntityData<FrostySdk.Ebx.RecordLinkTrackBaseData>
	{
		public new FrostySdk.Ebx.RecordLinkTrackBaseData Data => data as FrostySdk.Ebx.RecordLinkTrackBaseData;
		public override string DisplayName => "RecordLinkTrackBase";

		public RecordLinkTrackBase(FrostySdk.Ebx.RecordLinkTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

