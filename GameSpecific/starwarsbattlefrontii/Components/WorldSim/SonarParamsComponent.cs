
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SonarParamsComponentData))]
	public class SonarParamsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.SonarParamsComponentData>
	{
		public new FrostySdk.Ebx.SonarParamsComponentData Data => data as FrostySdk.Ebx.SonarParamsComponentData;
		public override string DisplayName => "SonarParamsComponent";

		public SonarParamsComponent(FrostySdk.Ebx.SonarParamsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

