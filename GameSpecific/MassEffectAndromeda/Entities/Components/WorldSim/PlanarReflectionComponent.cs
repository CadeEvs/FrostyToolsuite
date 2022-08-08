using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanarReflectionComponentData))]
	public class PlanarReflectionComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.PlanarReflectionComponentData>
	{
		public new FrostySdk.Ebx.PlanarReflectionComponentData Data => data as FrostySdk.Ebx.PlanarReflectionComponentData;
		public override string DisplayName => "PlanarReflectionComponent";

		public PlanarReflectionComponent(FrostySdk.Ebx.PlanarReflectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

