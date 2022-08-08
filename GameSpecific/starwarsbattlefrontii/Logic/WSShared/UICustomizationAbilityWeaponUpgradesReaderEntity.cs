using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradesReaderEntityData))]
	public class UICustomizationAbilityWeaponUpgradesReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradesReaderEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradesReaderEntityData Data => data as FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradesReaderEntityData;
		public override string DisplayName => "UICustomizationAbilityWeaponUpgradesReader";

		public UICustomizationAbilityWeaponUpgradesReaderEntity(FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradesReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

