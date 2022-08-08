
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActorPhysicsComponentData))]
	public class ActorPhysicsComponent : StaticModelPhysicsComponent, IEntityData<FrostySdk.Ebx.ActorPhysicsComponentData>
	{
		public new FrostySdk.Ebx.ActorPhysicsComponentData Data => data as FrostySdk.Ebx.ActorPhysicsComponentData;
		public override string DisplayName => "ActorPhysicsComponent";

		public ActorPhysicsComponent(FrostySdk.Ebx.ActorPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

