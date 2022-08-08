using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleAbilityDebugLogEntityData))]
	public class VehicleAbilityDebugLogEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VehicleAbilityDebugLogEntityData>
	{
		public new FrostySdk.Ebx.VehicleAbilityDebugLogEntityData Data => data as FrostySdk.Ebx.VehicleAbilityDebugLogEntityData;
		public override string DisplayName => "VehicleAbilityDebugLog";

		public VehicleAbilityDebugLogEntity(FrostySdk.Ebx.VehicleAbilityDebugLogEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

