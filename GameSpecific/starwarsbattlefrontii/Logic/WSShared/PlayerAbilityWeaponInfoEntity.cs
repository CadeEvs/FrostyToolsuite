using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityWeaponInfoEntityData))]
	public class PlayerAbilityWeaponInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityWeaponInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityWeaponInfoEntityData Data => data as FrostySdk.Ebx.PlayerAbilityWeaponInfoEntityData;
		public override string DisplayName => "PlayerAbilityWeaponInfo";

		public PlayerAbilityWeaponInfoEntity(FrostySdk.Ebx.PlayerAbilityWeaponInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

