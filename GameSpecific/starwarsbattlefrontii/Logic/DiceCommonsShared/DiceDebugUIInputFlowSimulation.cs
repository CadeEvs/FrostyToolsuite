using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceDebugUIInputFlowSimulationData))]
	public class DiceDebugUIInputFlowSimulation : LogicEntity, IEntityData<FrostySdk.Ebx.DiceDebugUIInputFlowSimulationData>
	{
		public new FrostySdk.Ebx.DiceDebugUIInputFlowSimulationData Data => data as FrostySdk.Ebx.DiceDebugUIInputFlowSimulationData;
		public override string DisplayName => "DiceDebugUIInputFlowSimulation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DiceDebugUIInputFlowSimulation(FrostySdk.Ebx.DiceDebugUIInputFlowSimulationData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

