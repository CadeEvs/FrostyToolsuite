
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShadowsComponentData))]
	public class ShadowsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ShadowsComponentData>
	{
		public new FrostySdk.Ebx.ShadowsComponentData Data => data as FrostySdk.Ebx.ShadowsComponentData;
		public override string DisplayName => "ShadowsComponent";

		public ShadowsComponent(FrostySdk.Ebx.ShadowsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

