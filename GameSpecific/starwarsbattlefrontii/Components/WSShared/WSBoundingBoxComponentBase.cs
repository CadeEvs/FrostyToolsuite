
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSBoundingBoxComponentBaseData))]
	public class WSBoundingBoxComponentBase : GameComponent, IEntityData<FrostySdk.Ebx.WSBoundingBoxComponentBaseData>
	{
		public new FrostySdk.Ebx.WSBoundingBoxComponentBaseData Data => data as FrostySdk.Ebx.WSBoundingBoxComponentBaseData;
		public override string DisplayName => "WSBoundingBoxComponentBase";

		public WSBoundingBoxComponentBase(FrostySdk.Ebx.WSBoundingBoxComponentBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

