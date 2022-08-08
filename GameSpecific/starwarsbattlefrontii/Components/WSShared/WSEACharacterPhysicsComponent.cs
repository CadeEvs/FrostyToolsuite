
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSEACharacterPhysicsComponentData))]
	public class WSEACharacterPhysicsComponent : EACharacterPhysicsComponent, IEntityData<FrostySdk.Ebx.WSEACharacterPhysicsComponentData>
	{
		public new FrostySdk.Ebx.WSEACharacterPhysicsComponentData Data => data as FrostySdk.Ebx.WSEACharacterPhysicsComponentData;
		public override string DisplayName => "WSEACharacterPhysicsComponent";

		public WSEACharacterPhysicsComponent(FrostySdk.Ebx.WSEACharacterPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

