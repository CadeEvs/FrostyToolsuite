using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationLoadoutSetSlotEntityData))]
	public class UICustomizationLoadoutSetSlotEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationLoadoutSetSlotEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationLoadoutSetSlotEntityData Data => data as FrostySdk.Ebx.UICustomizationLoadoutSetSlotEntityData;
		public override string DisplayName => "UICustomizationLoadoutSetSlot";

		public UICustomizationLoadoutSetSlotEntity(FrostySdk.Ebx.UICustomizationLoadoutSetSlotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

