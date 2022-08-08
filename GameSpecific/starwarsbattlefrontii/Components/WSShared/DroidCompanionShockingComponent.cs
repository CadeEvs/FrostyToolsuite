
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionShockingComponentData))]
	public class DroidCompanionShockingComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionShockingComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionShockingComponentData Data => data as FrostySdk.Ebx.DroidCompanionShockingComponentData;
		public override string DisplayName => "DroidCompanionShockingComponent";

		public DroidCompanionShockingComponent(FrostySdk.Ebx.DroidCompanionShockingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

