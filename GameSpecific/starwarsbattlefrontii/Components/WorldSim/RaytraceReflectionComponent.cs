
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RaytraceReflectionComponentData))]
	public class RaytraceReflectionComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.RaytraceReflectionComponentData>
	{
		public new FrostySdk.Ebx.RaytraceReflectionComponentData Data => data as FrostySdk.Ebx.RaytraceReflectionComponentData;
		public override string DisplayName => "RaytraceReflectionComponent";

		public RaytraceReflectionComponent(FrostySdk.Ebx.RaytraceReflectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

