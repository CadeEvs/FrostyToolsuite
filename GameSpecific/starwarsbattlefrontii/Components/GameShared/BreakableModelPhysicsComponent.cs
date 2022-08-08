
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BreakableModelPhysicsComponentData))]
	public class BreakableModelPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.BreakableModelPhysicsComponentData>
	{
		public new FrostySdk.Ebx.BreakableModelPhysicsComponentData Data => data as FrostySdk.Ebx.BreakableModelPhysicsComponentData;
		public override string DisplayName => "BreakableModelPhysicsComponent";

		public BreakableModelPhysicsComponent(FrostySdk.Ebx.BreakableModelPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

