
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LensReflectionComponentData))]
	public class LensReflectionComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.LensReflectionComponentData>
	{
		public new FrostySdk.Ebx.LensReflectionComponentData Data => data as FrostySdk.Ebx.LensReflectionComponentData;
		public override string DisplayName => "LensReflectionComponent";

		public LensReflectionComponent(FrostySdk.Ebx.LensReflectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

