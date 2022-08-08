using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterOceanSimulationEntityData))]
	public class WaterOceanSimulationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WaterOceanSimulationEntityData>
	{
		public new FrostySdk.Ebx.WaterOceanSimulationEntityData Data => data as FrostySdk.Ebx.WaterOceanSimulationEntityData;
		public override string DisplayName => "WaterOceanSimulation";

		public WaterOceanSimulationEntity(FrostySdk.Ebx.WaterOceanSimulationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

