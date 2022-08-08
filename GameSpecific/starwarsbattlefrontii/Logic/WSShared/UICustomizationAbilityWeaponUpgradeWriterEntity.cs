using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradeWriterEntityData))]
	public class UICustomizationAbilityWeaponUpgradeWriterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradeWriterEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradeWriterEntityData Data => data as FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradeWriterEntityData;
		public override string DisplayName => "UICustomizationAbilityWeaponUpgradeWriter";

		public UICustomizationAbilityWeaponUpgradeWriterEntity(FrostySdk.Ebx.UICustomizationAbilityWeaponUpgradeWriterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

