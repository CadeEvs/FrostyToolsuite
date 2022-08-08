
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttachTransformLayerData))]
	public class AttachTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.AttachTransformLayerData>
	{
		public new FrostySdk.Ebx.AttachTransformLayerData Data => data as FrostySdk.Ebx.AttachTransformLayerData;
		public override string DisplayName => "AttachTransformLayer";

		public AttachTransformLayer(FrostySdk.Ebx.AttachTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

