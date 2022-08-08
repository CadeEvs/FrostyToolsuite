using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestComponentEntityData))]
	public class TestComponentEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestComponentEntityData>
	{
		public new FrostySdk.Ebx.TestComponentEntityData Data => data as FrostySdk.Ebx.TestComponentEntityData;
		public override string DisplayName => "TestComponent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TestComponentEntity(FrostySdk.Ebx.TestComponentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

