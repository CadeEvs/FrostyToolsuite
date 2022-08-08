
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartPhysicsComponentData))]
	public class PartPhysicsComponent : PhysicsComponent, IEntityData<FrostySdk.Ebx.PartPhysicsComponentData>
	{
		public new FrostySdk.Ebx.PartPhysicsComponentData Data => data as FrostySdk.Ebx.PartPhysicsComponentData;
		public override string DisplayName => "PartPhysicsComponent";

		public PartPhysicsComponent(FrostySdk.Ebx.PartPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

