
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkyCelestialSingleComponentData))]
	public class SkyCelestialSingleComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.SkyCelestialSingleComponentData>
	{
		public new FrostySdk.Ebx.SkyCelestialSingleComponentData Data => data as FrostySdk.Ebx.SkyCelestialSingleComponentData;
		public override string DisplayName => "SkyCelestialSingleComponent";

		public SkyCelestialSingleComponent(FrostySdk.Ebx.SkyCelestialSingleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

