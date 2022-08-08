using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicAOComponentData))]
	public class DynamicAOComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.DynamicAOComponentData>
	{
		public new FrostySdk.Ebx.DynamicAOComponentData Data => data as FrostySdk.Ebx.DynamicAOComponentData;
		public override string DisplayName => "DynamicAOComponent";

		public DynamicAOComponent(FrostySdk.Ebx.DynamicAOComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

