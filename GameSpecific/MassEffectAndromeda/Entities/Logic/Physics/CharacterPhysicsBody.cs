using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterPhysicsBodyData))]
	public class CharacterPhysicsBody : PhysicsBody, IEntityData<FrostySdk.Ebx.CharacterPhysicsBodyData>
	{
		public new FrostySdk.Ebx.CharacterPhysicsBodyData Data => data as FrostySdk.Ebx.CharacterPhysicsBodyData;
		public override string DisplayName => "CharacterPhysicsBody";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CharacterPhysicsBody(FrostySdk.Ebx.CharacterPhysicsBodyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

