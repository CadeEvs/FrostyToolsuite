
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTAnimatableCameraTransformLayerData))]
	public class ANTAnimatableCameraTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.ANTAnimatableCameraTransformLayerData>
	{
		public new FrostySdk.Ebx.ANTAnimatableCameraTransformLayerData Data => data as FrostySdk.Ebx.ANTAnimatableCameraTransformLayerData;
		public override string DisplayName => "ANTAnimatableCameraTransformLayer";

		public ANTAnimatableCameraTransformLayer(FrostySdk.Ebx.ANTAnimatableCameraTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

