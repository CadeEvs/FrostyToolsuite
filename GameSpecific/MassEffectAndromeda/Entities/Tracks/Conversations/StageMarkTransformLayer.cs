
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StageMarkTransformLayerData))]
	public class StageMarkTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.StageMarkTransformLayerData>
	{
		public new FrostySdk.Ebx.StageMarkTransformLayerData Data => data as FrostySdk.Ebx.StageMarkTransformLayerData;
		public override string DisplayName => "StageMarkTransformLayer";

		public StageMarkTransformLayer(FrostySdk.Ebx.StageMarkTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

