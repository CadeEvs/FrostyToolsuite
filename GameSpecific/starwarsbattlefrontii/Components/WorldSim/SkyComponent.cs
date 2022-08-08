
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkyComponentData))]
	public class SkyComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.SkyComponentData>
	{
		public new FrostySdk.Ebx.SkyComponentData Data => data as FrostySdk.Ebx.SkyComponentData;
		public override string DisplayName => "SkyComponent";

		public SkyComponent(FrostySdk.Ebx.SkyComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

