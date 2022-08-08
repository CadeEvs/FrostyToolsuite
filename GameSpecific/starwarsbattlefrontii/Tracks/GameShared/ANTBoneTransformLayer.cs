
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTBoneTransformLayerData))]
	public class ANTBoneTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.ANTBoneTransformLayerData>
	{
		public new FrostySdk.Ebx.ANTBoneTransformLayerData Data => data as FrostySdk.Ebx.ANTBoneTransformLayerData;
		public override string DisplayName => "ANTBoneTransformLayer";

		public ANTBoneTransformLayer(FrostySdk.Ebx.ANTBoneTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

