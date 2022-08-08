
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CloudComponentData))]
	public class CloudComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.CloudComponentData>
	{
		public new FrostySdk.Ebx.CloudComponentData Data => data as FrostySdk.Ebx.CloudComponentData;
		public override string DisplayName => "CloudComponent";

		public CloudComponent(FrostySdk.Ebx.CloudComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

