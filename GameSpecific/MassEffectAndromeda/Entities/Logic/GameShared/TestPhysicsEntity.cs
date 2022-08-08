using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestPhysicsEntityData))]
	public class TestPhysicsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestPhysicsEntityData>
	{
		public new FrostySdk.Ebx.TestPhysicsEntityData Data => data as FrostySdk.Ebx.TestPhysicsEntityData;
		public override string DisplayName => "TestPhysics";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TestPhysicsEntity(FrostySdk.Ebx.TestPhysicsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

