
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MissilePhysicsComponentData))]
	public class MissilePhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.MissilePhysicsComponentData>
	{
		public new FrostySdk.Ebx.MissilePhysicsComponentData Data => data as FrostySdk.Ebx.MissilePhysicsComponentData;
		public override string DisplayName => "MissilePhysicsComponent";

		public MissilePhysicsComponent(FrostySdk.Ebx.MissilePhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

