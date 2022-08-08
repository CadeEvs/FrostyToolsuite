using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugUIInputActionSimulationEntityData))]
	public class DebugUIInputActionSimulationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DebugUIInputActionSimulationEntityData>
	{
		public new FrostySdk.Ebx.DebugUIInputActionSimulationEntityData Data => data as FrostySdk.Ebx.DebugUIInputActionSimulationEntityData;
		public override string DisplayName => "DebugUIInputActionSimulation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DebugUIInputActionSimulationEntity(FrostySdk.Ebx.DebugUIInputActionSimulationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

