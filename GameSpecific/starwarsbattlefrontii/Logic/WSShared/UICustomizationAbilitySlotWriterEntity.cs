using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationAbilitySlotWriterEntityData))]
	public class UICustomizationAbilitySlotWriterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationAbilitySlotWriterEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationAbilitySlotWriterEntityData Data => data as FrostySdk.Ebx.UICustomizationAbilitySlotWriterEntityData;
		public override string DisplayName => "UICustomizationAbilitySlotWriter";

		public UICustomizationAbilitySlotWriterEntity(FrostySdk.Ebx.UICustomizationAbilitySlotWriterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

