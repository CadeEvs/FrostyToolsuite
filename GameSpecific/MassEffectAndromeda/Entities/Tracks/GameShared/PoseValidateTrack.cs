
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PoseValidateTrackData))]
	public class PoseValidateTrack : ValidateLinkTrackBase, IEntityData<FrostySdk.Ebx.PoseValidateTrackData>
	{
		public new FrostySdk.Ebx.PoseValidateTrackData Data => data as FrostySdk.Ebx.PoseValidateTrackData;
		public override string DisplayName => "PoseValidateTrack";

		public PoseValidateTrack(FrostySdk.Ebx.PoseValidateTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

