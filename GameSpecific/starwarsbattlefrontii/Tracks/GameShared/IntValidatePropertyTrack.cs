
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntValidatePropertyTrackData))]
	public class IntValidatePropertyTrack : ValidatePropertyTrackBase, IEntityData<FrostySdk.Ebx.IntValidatePropertyTrackData>
	{
		public new FrostySdk.Ebx.IntValidatePropertyTrackData Data => data as FrostySdk.Ebx.IntValidatePropertyTrackData;
		public override string DisplayName => "IntValidatePropertyTrack";

		public IntValidatePropertyTrack(FrostySdk.Ebx.IntValidatePropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

