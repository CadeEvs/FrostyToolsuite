
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllablePhysicsComponentData))]
	public class ControllablePhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.ControllablePhysicsComponentData>
	{
		public new FrostySdk.Ebx.ControllablePhysicsComponentData Data => data as FrostySdk.Ebx.ControllablePhysicsComponentData;
		public override string DisplayName => "ControllablePhysicsComponent";

		public ControllablePhysicsComponent(FrostySdk.Ebx.ControllablePhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

