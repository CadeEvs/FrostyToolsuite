using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimingSimulationEntityData))]
	public class AimingSimulationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AimingSimulationEntityData>
	{
		public new FrostySdk.Ebx.AimingSimulationEntityData Data => data as FrostySdk.Ebx.AimingSimulationEntityData;
		public override string DisplayName => "AimingSimulation";

		public AimingSimulationEntity(FrostySdk.Ebx.AimingSimulationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

