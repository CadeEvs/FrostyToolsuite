using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestCollisionEventEntityData))]
	public class TestCollisionEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestCollisionEventEntityData>
	{
		public new FrostySdk.Ebx.TestCollisionEventEntityData Data => data as FrostySdk.Ebx.TestCollisionEventEntityData;
		public override string DisplayName => "TestCollisionEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TestCollisionEventEntity(FrostySdk.Ebx.TestCollisionEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

