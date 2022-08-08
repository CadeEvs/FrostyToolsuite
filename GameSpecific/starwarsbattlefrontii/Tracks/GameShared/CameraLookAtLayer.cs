
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraLookAtLayerData))]
	public class CameraLookAtLayer : TransformLayer, IEntityData<FrostySdk.Ebx.CameraLookAtLayerData>
	{
		public new FrostySdk.Ebx.CameraLookAtLayerData Data => data as FrostySdk.Ebx.CameraLookAtLayerData;
		public override string DisplayName => "CameraLookAtLayer";

		public CameraLookAtLayer(FrostySdk.Ebx.CameraLookAtLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

