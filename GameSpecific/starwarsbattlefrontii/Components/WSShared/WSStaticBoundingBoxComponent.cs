
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSStaticBoundingBoxComponentData))]
	public class WSStaticBoundingBoxComponent : WSBoundingBoxComponentBase, IEntityData<FrostySdk.Ebx.WSStaticBoundingBoxComponentData>
	{
		public new FrostySdk.Ebx.WSStaticBoundingBoxComponentData Data => data as FrostySdk.Ebx.WSStaticBoundingBoxComponentData;
		public override string DisplayName => "WSStaticBoundingBoxComponent";

		public WSStaticBoundingBoxComponent(FrostySdk.Ebx.WSStaticBoundingBoxComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

