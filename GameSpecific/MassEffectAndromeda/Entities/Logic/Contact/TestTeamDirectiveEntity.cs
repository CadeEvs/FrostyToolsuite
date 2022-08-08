using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestTeamDirectiveEntityData))]
	public class TestTeamDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestTeamDirectiveEntityData>
	{
		public new FrostySdk.Ebx.TestTeamDirectiveEntityData Data => data as FrostySdk.Ebx.TestTeamDirectiveEntityData;
		public override string DisplayName => "TestTeamDirective";

		public TestTeamDirectiveEntity(FrostySdk.Ebx.TestTeamDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

