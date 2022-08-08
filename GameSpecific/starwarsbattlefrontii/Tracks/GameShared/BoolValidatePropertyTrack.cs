
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolValidatePropertyTrackData))]
	public class BoolValidatePropertyTrack : ValidatePropertyTrackBase, IEntityData<FrostySdk.Ebx.BoolValidatePropertyTrackData>
	{
		public new FrostySdk.Ebx.BoolValidatePropertyTrackData Data => data as FrostySdk.Ebx.BoolValidatePropertyTrackData;
		public override string DisplayName => "BoolValidatePropertyTrack";

		public BoolValidatePropertyTrack(FrostySdk.Ebx.BoolValidatePropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

