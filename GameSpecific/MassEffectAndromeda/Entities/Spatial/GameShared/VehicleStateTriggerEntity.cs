using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleStateTriggerEntityData))]
	public class VehicleStateTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.VehicleStateTriggerEntityData>
	{
		public new FrostySdk.Ebx.VehicleStateTriggerEntityData Data => data as FrostySdk.Ebx.VehicleStateTriggerEntityData;

		public VehicleStateTriggerEntity(FrostySdk.Ebx.VehicleStateTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

