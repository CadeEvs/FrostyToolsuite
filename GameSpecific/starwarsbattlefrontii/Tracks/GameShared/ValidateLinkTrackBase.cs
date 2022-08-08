
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ValidateLinkTrackBaseData))]
	public class ValidateLinkTrackBase : LinkTrack, IEntityData<FrostySdk.Ebx.ValidateLinkTrackBaseData>
	{
		public new FrostySdk.Ebx.ValidateLinkTrackBaseData Data => data as FrostySdk.Ebx.ValidateLinkTrackBaseData;
		public override string DisplayName => "ValidateLinkTrackBase";

		public ValidateLinkTrackBase(FrostySdk.Ebx.ValidateLinkTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

