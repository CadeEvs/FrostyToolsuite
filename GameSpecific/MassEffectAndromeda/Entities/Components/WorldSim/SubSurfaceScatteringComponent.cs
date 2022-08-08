using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubSurfaceScatteringComponentData))]
	public class SubSurfaceScatteringComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.SubSurfaceScatteringComponentData>
	{
		public new FrostySdk.Ebx.SubSurfaceScatteringComponentData Data => data as FrostySdk.Ebx.SubSurfaceScatteringComponentData;
		public override string DisplayName => "SubSurfaceScatteringComponent";

		public SubSurfaceScatteringComponent(FrostySdk.Ebx.SubSurfaceScatteringComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

