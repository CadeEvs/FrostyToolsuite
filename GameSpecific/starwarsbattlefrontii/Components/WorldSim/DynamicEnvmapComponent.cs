
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicEnvmapComponentData))]
	public class DynamicEnvmapComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.DynamicEnvmapComponentData>
	{
		public new FrostySdk.Ebx.DynamicEnvmapComponentData Data => data as FrostySdk.Ebx.DynamicEnvmapComponentData;
		public override string DisplayName => "DynamicEnvmapComponent";

		public DynamicEnvmapComponent(FrostySdk.Ebx.DynamicEnvmapComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

