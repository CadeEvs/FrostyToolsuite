
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShaderParameterComponentData))]
	public class ShaderParameterComponent : GameComponent, IEntityData<FrostySdk.Ebx.ShaderParameterComponentData>
	{
		public new FrostySdk.Ebx.ShaderParameterComponentData Data => data as FrostySdk.Ebx.ShaderParameterComponentData;
		public override string DisplayName => "ShaderParameterComponent";

		public ShaderParameterComponent(FrostySdk.Ebx.ShaderParameterComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

