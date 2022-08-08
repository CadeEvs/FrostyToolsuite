
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PickupPhysicsComponentData))]
	public class PickupPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.PickupPhysicsComponentData>
	{
		public new FrostySdk.Ebx.PickupPhysicsComponentData Data => data as FrostySdk.Ebx.PickupPhysicsComponentData;
		public override string DisplayName => "PickupPhysicsComponent";

		public PickupPhysicsComponent(FrostySdk.Ebx.PickupPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

