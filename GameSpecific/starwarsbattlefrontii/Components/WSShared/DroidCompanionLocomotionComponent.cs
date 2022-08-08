
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionLocomotionComponentData))]
	public class DroidCompanionLocomotionComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionLocomotionComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionLocomotionComponentData Data => data as FrostySdk.Ebx.DroidCompanionLocomotionComponentData;
		public override string DisplayName => "DroidCompanionLocomotionComponent";

		public DroidCompanionLocomotionComponent(FrostySdk.Ebx.DroidCompanionLocomotionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

