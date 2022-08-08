
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShieldPhysicsComponentData))]
	public class ShieldPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.ShieldPhysicsComponentData>
	{
		public new FrostySdk.Ebx.ShieldPhysicsComponentData Data => data as FrostySdk.Ebx.ShieldPhysicsComponentData;
		public override string DisplayName => "ShieldPhysicsComponent";

		public ShieldPhysicsComponent(FrostySdk.Ebx.ShieldPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

