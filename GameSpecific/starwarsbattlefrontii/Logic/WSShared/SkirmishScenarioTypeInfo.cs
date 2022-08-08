using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkirmishScenarioTypeInfoData))]
	public class SkirmishScenarioTypeInfo : LogicEntity, IEntityData<FrostySdk.Ebx.SkirmishScenarioTypeInfoData>
	{
		public new FrostySdk.Ebx.SkirmishScenarioTypeInfoData Data => data as FrostySdk.Ebx.SkirmishScenarioTypeInfoData;
		public override string DisplayName => "SkirmishScenarioTypeInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SkirmishScenarioTypeInfo(FrostySdk.Ebx.SkirmishScenarioTypeInfoData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

