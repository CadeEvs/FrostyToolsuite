
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterPhysicsComponentData))]
	public class StarfighterPhysicsComponent : VehiclePhysicsComponent, IEntityData<FrostySdk.Ebx.StarfighterPhysicsComponentData>
	{
		public new FrostySdk.Ebx.StarfighterPhysicsComponentData Data => data as FrostySdk.Ebx.StarfighterPhysicsComponentData;
		public override string DisplayName => "StarfighterPhysicsComponent";

		public StarfighterPhysicsComponent(FrostySdk.Ebx.StarfighterPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

