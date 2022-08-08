using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSWeaponModifierUnlocksEntityData))]
	public class WSWeaponModifierUnlocksEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSWeaponModifierUnlocksEntityData>
	{
		public new FrostySdk.Ebx.WSWeaponModifierUnlocksEntityData Data => data as FrostySdk.Ebx.WSWeaponModifierUnlocksEntityData;
		public override string DisplayName => "WSWeaponModifierUnlocks";

		public WSWeaponModifierUnlocksEntity(FrostySdk.Ebx.WSWeaponModifierUnlocksEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

