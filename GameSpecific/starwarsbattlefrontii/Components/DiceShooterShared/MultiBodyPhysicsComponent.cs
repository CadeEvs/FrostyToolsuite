
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MultiBodyPhysicsComponentData))]
	public class MultiBodyPhysicsComponent : VehiclePhysicsComponent, IEntityData<FrostySdk.Ebx.MultiBodyPhysicsComponentData>
	{
		public new FrostySdk.Ebx.MultiBodyPhysicsComponentData Data => data as FrostySdk.Ebx.MultiBodyPhysicsComponentData;
		public override string DisplayName => "MultiBodyPhysicsComponent";

		public MultiBodyPhysicsComponent(FrostySdk.Ebx.MultiBodyPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

