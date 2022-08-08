
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathTransformLayerData))]
	public class PathTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.PathTransformLayerData>
	{
		public new FrostySdk.Ebx.PathTransformLayerData Data => data as FrostySdk.Ebx.PathTransformLayerData;
		public override string DisplayName => "PathTransformLayer";

		public PathTransformLayer(FrostySdk.Ebx.PathTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

