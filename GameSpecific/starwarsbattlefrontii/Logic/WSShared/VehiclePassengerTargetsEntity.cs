using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehiclePassengerTargetsEntityData))]
	public class VehiclePassengerTargetsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VehiclePassengerTargetsEntityData>
	{
		public new FrostySdk.Ebx.VehiclePassengerTargetsEntityData Data => data as FrostySdk.Ebx.VehiclePassengerTargetsEntityData;
		public override string DisplayName => "VehiclePassengerTargets";

		public VehiclePassengerTargetsEntity(FrostySdk.Ebx.VehiclePassengerTargetsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

