using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenSpaceRaytraceComponentData))]
	public class ScreenSpaceRaytraceComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ScreenSpaceRaytraceComponentData>
	{
		public new FrostySdk.Ebx.ScreenSpaceRaytraceComponentData Data => data as FrostySdk.Ebx.ScreenSpaceRaytraceComponentData;
		public override string DisplayName => "ScreenSpaceRaytraceComponent";

		public ScreenSpaceRaytraceComponent(FrostySdk.Ebx.ScreenSpaceRaytraceComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

