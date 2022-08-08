using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationAbilityActiveUpgradesReaderEntityData))]
	public class UICustomizationAbilityActiveUpgradesReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationAbilityActiveUpgradesReaderEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationAbilityActiveUpgradesReaderEntityData Data => data as FrostySdk.Ebx.UICustomizationAbilityActiveUpgradesReaderEntityData;
		public override string DisplayName => "UICustomizationAbilityActiveUpgradesReader";

		public UICustomizationAbilityActiveUpgradesReaderEntity(FrostySdk.Ebx.UICustomizationAbilityActiveUpgradesReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

