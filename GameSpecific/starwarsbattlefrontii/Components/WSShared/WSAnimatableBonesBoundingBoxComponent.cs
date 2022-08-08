
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSAnimatableBonesBoundingBoxComponentData))]
	public class WSAnimatableBonesBoundingBoxComponent : WSBoundingBoxComponentBase, IEntityData<FrostySdk.Ebx.WSAnimatableBonesBoundingBoxComponentData>
	{
		public new FrostySdk.Ebx.WSAnimatableBonesBoundingBoxComponentData Data => data as FrostySdk.Ebx.WSAnimatableBonesBoundingBoxComponentData;
		public override string DisplayName => "WSAnimatableBonesBoundingBoxComponent";

		public WSAnimatableBonesBoundingBoxComponent(FrostySdk.Ebx.WSAnimatableBonesBoundingBoxComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

