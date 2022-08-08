
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StartingLocationTransformLayerData))]
	public class StartingLocationTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.StartingLocationTransformLayerData>
	{
		public new FrostySdk.Ebx.StartingLocationTransformLayerData Data => data as FrostySdk.Ebx.StartingLocationTransformLayerData;
		public override string DisplayName => "StartingLocationTransformLayer";

		public StartingLocationTransformLayer(FrostySdk.Ebx.StartingLocationTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

