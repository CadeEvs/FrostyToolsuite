using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEEquipmentVisibilityEntityData))]
	public class MEEquipmentVisibilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEEquipmentVisibilityEntityData>
	{
		public new FrostySdk.Ebx.MEEquipmentVisibilityEntityData Data => data as FrostySdk.Ebx.MEEquipmentVisibilityEntityData;
		public override string DisplayName => "MEEquipmentVisibility";

		public MEEquipmentVisibilityEntity(FrostySdk.Ebx.MEEquipmentVisibilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

