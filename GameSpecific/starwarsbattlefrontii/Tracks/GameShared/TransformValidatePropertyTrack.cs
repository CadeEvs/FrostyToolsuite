
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformValidatePropertyTrackData))]
	public class TransformValidatePropertyTrack : ValidatePropertyTrackBase, IEntityData<FrostySdk.Ebx.TransformValidatePropertyTrackData>
	{
		public new FrostySdk.Ebx.TransformValidatePropertyTrackData Data => data as FrostySdk.Ebx.TransformValidatePropertyTrackData;
		public override string DisplayName => "TransformValidatePropertyTrack";

		public TransformValidatePropertyTrack(FrostySdk.Ebx.TransformValidatePropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

