using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEEquipmentPowerSlotEntityData))]
	public class MEEquipmentPowerSlotEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEEquipmentPowerSlotEntityData>
	{
		public new FrostySdk.Ebx.MEEquipmentPowerSlotEntityData Data => data as FrostySdk.Ebx.MEEquipmentPowerSlotEntityData;
		public override string DisplayName => "MEEquipmentPowerSlot";

		public MEEquipmentPowerSlotEntity(FrostySdk.Ebx.MEEquipmentPowerSlotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

