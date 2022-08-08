
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroundAttachTransformLayerData))]
	public class GroundAttachTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.GroundAttachTransformLayerData>
	{
		public new FrostySdk.Ebx.GroundAttachTransformLayerData Data => data as FrostySdk.Ebx.GroundAttachTransformLayerData;
		public override string DisplayName => "GroundAttachTransformLayer";

		public GroundAttachTransformLayer(FrostySdk.Ebx.GroundAttachTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

