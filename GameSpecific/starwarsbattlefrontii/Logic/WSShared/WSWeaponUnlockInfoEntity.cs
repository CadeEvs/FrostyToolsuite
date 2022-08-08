using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSWeaponUnlockInfoEntityData))]
	public class WSWeaponUnlockInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSWeaponUnlockInfoEntityData>
	{
		public new FrostySdk.Ebx.WSWeaponUnlockInfoEntityData Data => data as FrostySdk.Ebx.WSWeaponUnlockInfoEntityData;
		public override string DisplayName => "WSWeaponUnlockInfo";

		public WSWeaponUnlockInfoEntity(FrostySdk.Ebx.WSWeaponUnlockInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

