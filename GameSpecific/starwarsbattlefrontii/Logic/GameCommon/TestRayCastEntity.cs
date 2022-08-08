using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestRayCastEntityData))]
	public class TestRayCastEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestRayCastEntityData>
	{
		public new FrostySdk.Ebx.TestRayCastEntityData Data => data as FrostySdk.Ebx.TestRayCastEntityData;
		public override string DisplayName => "TestRayCast";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TestRayCastEntity(FrostySdk.Ebx.TestRayCastEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

