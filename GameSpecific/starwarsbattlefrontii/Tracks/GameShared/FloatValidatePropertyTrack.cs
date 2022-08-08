
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatValidatePropertyTrackData))]
	public class FloatValidatePropertyTrack : ValidatePropertyTrackBase, IEntityData<FrostySdk.Ebx.FloatValidatePropertyTrackData>
	{
		public new FrostySdk.Ebx.FloatValidatePropertyTrackData Data => data as FrostySdk.Ebx.FloatValidatePropertyTrackData;
		public override string DisplayName => "FloatValidatePropertyTrack";

		public FloatValidatePropertyTrack(FrostySdk.Ebx.FloatValidatePropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

