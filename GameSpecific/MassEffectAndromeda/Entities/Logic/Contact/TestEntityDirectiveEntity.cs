using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestEntityDirectiveEntityData))]
	public class TestEntityDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestEntityDirectiveEntityData>
	{
		public new FrostySdk.Ebx.TestEntityDirectiveEntityData Data => data as FrostySdk.Ebx.TestEntityDirectiveEntityData;
		public override string DisplayName => "TestEntityDirective";

		public TestEntityDirectiveEntity(FrostySdk.Ebx.TestEntityDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

