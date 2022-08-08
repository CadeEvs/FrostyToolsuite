
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionTargetingComponentData))]
	public class DroidCompanionTargetingComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionTargetingComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionTargetingComponentData Data => data as FrostySdk.Ebx.DroidCompanionTargetingComponentData;
		public override string DisplayName => "DroidCompanionTargetingComponent";

		public DroidCompanionTargetingComponent(FrostySdk.Ebx.DroidCompanionTargetingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

