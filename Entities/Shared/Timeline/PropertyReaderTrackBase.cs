
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyReaderTrackBaseData))]
	public class PropertyReaderTrackBase : SchematicPinTrack, IEntityData<FrostySdk.Ebx.PropertyReaderTrackBaseData>
	{
		public new FrostySdk.Ebx.PropertyReaderTrackBaseData Data => data as FrostySdk.Ebx.PropertyReaderTrackBaseData;
		public override string DisplayName => "PropertyReaderTrackBase";

		public PropertyReaderTrackBase(FrostySdk.Ebx.PropertyReaderTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

