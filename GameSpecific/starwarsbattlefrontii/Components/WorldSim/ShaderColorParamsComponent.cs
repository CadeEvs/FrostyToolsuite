
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShaderColorParamsComponentData))]
	public class ShaderColorParamsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ShaderColorParamsComponentData>
	{
		public new FrostySdk.Ebx.ShaderColorParamsComponentData Data => data as FrostySdk.Ebx.ShaderColorParamsComponentData;
		public override string DisplayName => "ShaderColorParamsComponent";

		public ShaderColorParamsComponent(FrostySdk.Ebx.ShaderColorParamsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

