
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTTrajectoryTransformLayerData))]
	public class ANTTrajectoryTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.ANTTrajectoryTransformLayerData>
	{
		public new FrostySdk.Ebx.ANTTrajectoryTransformLayerData Data => data as FrostySdk.Ebx.ANTTrajectoryTransformLayerData;
		public override string DisplayName => "ANTTrajectoryTransformLayer";

		public ANTTrajectoryTransformLayer(FrostySdk.Ebx.ANTTrajectoryTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

