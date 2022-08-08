
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OffsetPresetTransformLayerData))]
	public class OffsetPresetTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.OffsetPresetTransformLayerData>
	{
		public new FrostySdk.Ebx.OffsetPresetTransformLayerData Data => data as FrostySdk.Ebx.OffsetPresetTransformLayerData;
		public override string DisplayName => "OffsetPresetTransformLayer";

		public OffsetPresetTransformLayer(FrostySdk.Ebx.OffsetPresetTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

