using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingAIVehicleAimingInterfaceEntityData))]
	public class TargetingAIVehicleAimingInterfaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TargetingAIVehicleAimingInterfaceEntityData>
	{
		public new FrostySdk.Ebx.TargetingAIVehicleAimingInterfaceEntityData Data => data as FrostySdk.Ebx.TargetingAIVehicleAimingInterfaceEntityData;
		public override string DisplayName => "TargetingAIVehicleAimingInterface";

		public TargetingAIVehicleAimingInterfaceEntity(FrostySdk.Ebx.TargetingAIVehicleAimingInterfaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

