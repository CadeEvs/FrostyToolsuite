
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionHackingComponentData))]
	public class DroidCompanionHackingComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionHackingComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionHackingComponentData Data => data as FrostySdk.Ebx.DroidCompanionHackingComponentData;
		public override string DisplayName => "DroidCompanionHackingComponent";

		public DroidCompanionHackingComponent(FrostySdk.Ebx.DroidCompanionHackingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

