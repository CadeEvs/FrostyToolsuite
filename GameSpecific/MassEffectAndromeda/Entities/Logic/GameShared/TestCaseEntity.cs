using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestCaseEntityData))]
	public class TestCaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestCaseEntityData>
	{
		public new FrostySdk.Ebx.TestCaseEntityData Data => data as FrostySdk.Ebx.TestCaseEntityData;
		public override string DisplayName => "TestCase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TestCaseEntity(FrostySdk.Ebx.TestCaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

