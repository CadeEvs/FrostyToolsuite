
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RagdollPhysicsComponentData))]
	public class RagdollPhysicsComponent : PhysicsComponent, IEntityData<FrostySdk.Ebx.RagdollPhysicsComponentData>
	{
		public new FrostySdk.Ebx.RagdollPhysicsComponentData Data => data as FrostySdk.Ebx.RagdollPhysicsComponentData;
		public override string DisplayName => "RagdollPhysicsComponent";

		public RagdollPhysicsComponent(FrostySdk.Ebx.RagdollPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

