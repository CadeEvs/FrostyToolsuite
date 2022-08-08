using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponAbilityMetaDataEntityData))]
	public class WeaponAbilityMetaDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeaponAbilityMetaDataEntityData>
	{
		public new FrostySdk.Ebx.WeaponAbilityMetaDataEntityData Data => data as FrostySdk.Ebx.WeaponAbilityMetaDataEntityData;
		public override string DisplayName => "WeaponAbilityMetaData";

		public WeaponAbilityMetaDataEntity(FrostySdk.Ebx.WeaponAbilityMetaDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

