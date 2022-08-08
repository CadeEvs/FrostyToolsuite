
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimAssistNodeSnapPointComponentData))]
	public class AimAssistNodeSnapPointComponent : GameComponent, IEntityData<FrostySdk.Ebx.AimAssistNodeSnapPointComponentData>
	{
		public new FrostySdk.Ebx.AimAssistNodeSnapPointComponentData Data => data as FrostySdk.Ebx.AimAssistNodeSnapPointComponentData;
		public override string DisplayName => "AimAssistNodeSnapPointComponent";

		public AimAssistNodeSnapPointComponent(FrostySdk.Ebx.AimAssistNodeSnapPointComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

