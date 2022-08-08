
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ValidatePropertyTrackBaseData))]
	public class ValidatePropertyTrackBase : PropertyReaderTrackBase, IEntityData<FrostySdk.Ebx.ValidatePropertyTrackBaseData>
	{
		public new FrostySdk.Ebx.ValidatePropertyTrackBaseData Data => data as FrostySdk.Ebx.ValidatePropertyTrackBaseData;
		public override string DisplayName => "ValidatePropertyTrackBase";

		public ValidatePropertyTrackBase(FrostySdk.Ebx.ValidatePropertyTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

