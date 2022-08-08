using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityWeaponUpgradeInfoEntityData))]
	public class PlayerAbilityWeaponUpgradeInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityWeaponUpgradeInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityWeaponUpgradeInfoEntityData Data => data as FrostySdk.Ebx.PlayerAbilityWeaponUpgradeInfoEntityData;
		public override string DisplayName => "PlayerAbilityWeaponUpgradeInfo";

		public PlayerAbilityWeaponUpgradeInfoEntity(FrostySdk.Ebx.PlayerAbilityWeaponUpgradeInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

