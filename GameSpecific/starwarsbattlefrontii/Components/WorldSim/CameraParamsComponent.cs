
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraParamsComponentData))]
	public class CameraParamsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.CameraParamsComponentData>
	{
		public new FrostySdk.Ebx.CameraParamsComponentData Data => data as FrostySdk.Ebx.CameraParamsComponentData;
		public override string DisplayName => "CameraParamsComponent";

		public CameraParamsComponent(FrostySdk.Ebx.CameraParamsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

