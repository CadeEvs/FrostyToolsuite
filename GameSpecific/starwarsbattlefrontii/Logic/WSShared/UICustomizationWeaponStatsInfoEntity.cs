using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationWeaponStatsInfoEntityData))]
	public class UICustomizationWeaponStatsInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationWeaponStatsInfoEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationWeaponStatsInfoEntityData Data => data as FrostySdk.Ebx.UICustomizationWeaponStatsInfoEntityData;
		public override string DisplayName => "UICustomizationWeaponStatsInfo";

		public UICustomizationWeaponStatsInfoEntity(FrostySdk.Ebx.UICustomizationWeaponStatsInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

