
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MotionBlurComponentData))]
	public class MotionBlurComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.MotionBlurComponentData>
	{
		public new FrostySdk.Ebx.MotionBlurComponentData Data => data as FrostySdk.Ebx.MotionBlurComponentData;
		public override string DisplayName => "MotionBlurComponent";

		public MotionBlurComponent(FrostySdk.Ebx.MotionBlurComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

