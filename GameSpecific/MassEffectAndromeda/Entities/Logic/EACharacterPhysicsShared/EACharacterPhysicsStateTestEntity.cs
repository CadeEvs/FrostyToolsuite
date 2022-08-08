using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EACharacterPhysicsStateTestEntityData))]
	public class EACharacterPhysicsStateTestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EACharacterPhysicsStateTestEntityData>
	{
		public new FrostySdk.Ebx.EACharacterPhysicsStateTestEntityData Data => data as FrostySdk.Ebx.EACharacterPhysicsStateTestEntityData;
		public override string DisplayName => "EACharacterPhysicsStateTest";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EACharacterPhysicsStateTestEntity(FrostySdk.Ebx.EACharacterPhysicsStateTestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

