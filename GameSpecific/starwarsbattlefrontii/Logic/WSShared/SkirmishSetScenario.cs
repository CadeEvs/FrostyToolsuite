using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkirmishSetScenarioData))]
	public class SkirmishSetScenario : LogicEntity, IEntityData<FrostySdk.Ebx.SkirmishSetScenarioData>
	{
		public new FrostySdk.Ebx.SkirmishSetScenarioData Data => data as FrostySdk.Ebx.SkirmishSetScenarioData;
		public override string DisplayName => "SkirmishSetScenario";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SkirmishSetScenario(FrostySdk.Ebx.SkirmishSetScenarioData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

