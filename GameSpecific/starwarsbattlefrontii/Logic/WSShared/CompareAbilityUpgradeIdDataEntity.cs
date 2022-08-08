using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareAbilityUpgradeIdDataEntityData))]
	public class CompareAbilityUpgradeIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareAbilityUpgradeIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareAbilityUpgradeIdDataEntityData Data => data as FrostySdk.Ebx.CompareAbilityUpgradeIdDataEntityData;
		public override string DisplayName => "CompareAbilityUpgradeIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareAbilityUpgradeIdDataEntity(FrostySdk.Ebx.CompareAbilityUpgradeIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

