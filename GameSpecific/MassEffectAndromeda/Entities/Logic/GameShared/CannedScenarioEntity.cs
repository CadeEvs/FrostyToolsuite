using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CannedScenarioEntityData))]
	public class CannedScenarioEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CannedScenarioEntityData>
	{
		public new FrostySdk.Ebx.CannedScenarioEntityData Data => data as FrostySdk.Ebx.CannedScenarioEntityData;
		public override string DisplayName => "CannedScenario";

		public CannedScenarioEntity(FrostySdk.Ebx.CannedScenarioEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

