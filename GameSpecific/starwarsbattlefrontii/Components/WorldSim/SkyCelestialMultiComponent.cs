
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkyCelestialMultiComponentData))]
	public class SkyCelestialMultiComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.SkyCelestialMultiComponentData>
	{
		public new FrostySdk.Ebx.SkyCelestialMultiComponentData Data => data as FrostySdk.Ebx.SkyCelestialMultiComponentData;
		public override string DisplayName => "SkyCelestialMultiComponent";

		public SkyCelestialMultiComponent(FrostySdk.Ebx.SkyCelestialMultiComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

