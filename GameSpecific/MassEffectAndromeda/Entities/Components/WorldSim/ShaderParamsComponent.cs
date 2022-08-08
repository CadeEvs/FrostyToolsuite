using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShaderParamsComponentData))]
	public class ShaderParamsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ShaderParamsComponentData>
	{
		public new FrostySdk.Ebx.ShaderParamsComponentData Data => data as FrostySdk.Ebx.ShaderParamsComponentData;
		public override string DisplayName => "ShaderParamsComponent";

		public ShaderParamsComponent(FrostySdk.Ebx.ShaderParamsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

