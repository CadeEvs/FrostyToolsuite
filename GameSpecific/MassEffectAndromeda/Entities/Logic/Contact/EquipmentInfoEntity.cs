using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EquipmentInfoEntityData))]
	public class EquipmentInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EquipmentInfoEntityData>
	{
		public new FrostySdk.Ebx.EquipmentInfoEntityData Data => data as FrostySdk.Ebx.EquipmentInfoEntityData;
		public override string DisplayName => "EquipmentInfo";

		public EquipmentInfoEntity(FrostySdk.Ebx.EquipmentInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

