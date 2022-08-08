
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterPhysicsComponentData))]
	public class CharacterPhysicsComponent : GameComponent, IEntityData<FrostySdk.Ebx.CharacterPhysicsComponentData>
	{
		public new FrostySdk.Ebx.CharacterPhysicsComponentData Data => data as FrostySdk.Ebx.CharacterPhysicsComponentData;
		public override string DisplayName => "CharacterPhysicsComponent";

		public CharacterPhysicsComponent(FrostySdk.Ebx.CharacterPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

