
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraAttachTransformLayerData))]
	public class CameraAttachTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.CameraAttachTransformLayerData>
	{
		public new FrostySdk.Ebx.CameraAttachTransformLayerData Data => data as FrostySdk.Ebx.CameraAttachTransformLayerData;
		public override string DisplayName => "CameraAttachTransformLayer";

		public CameraAttachTransformLayer(FrostySdk.Ebx.CameraAttachTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

