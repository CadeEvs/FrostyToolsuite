using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationCardEquipEntityData))]
	public class UICustomizationCardEquipEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationCardEquipEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationCardEquipEntityData Data => data as FrostySdk.Ebx.UICustomizationCardEquipEntityData;
		public override string DisplayName => "UICustomizationCardEquip";

		public UICustomizationCardEquipEntity(FrostySdk.Ebx.UICustomizationCardEquipEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

