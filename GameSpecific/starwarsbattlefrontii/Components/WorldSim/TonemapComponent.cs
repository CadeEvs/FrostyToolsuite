
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TonemapComponentData))]
	public class TonemapComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.TonemapComponentData>
	{
		public new FrostySdk.Ebx.TonemapComponentData Data => data as FrostySdk.Ebx.TonemapComponentData;
		public override string DisplayName => "TonemapComponent";

		public TonemapComponent(FrostySdk.Ebx.TonemapComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

