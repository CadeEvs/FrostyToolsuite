
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EACharacterPhysicsComponentData))]
	public class EACharacterPhysicsComponent : GameComponent, IEntityData<FrostySdk.Ebx.EACharacterPhysicsComponentData>
	{
		public new FrostySdk.Ebx.EACharacterPhysicsComponentData Data => data as FrostySdk.Ebx.EACharacterPhysicsComponentData;
		public override string DisplayName => "EACharacterPhysicsComponent";

		public EACharacterPhysicsComponent(FrostySdk.Ebx.EACharacterPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

