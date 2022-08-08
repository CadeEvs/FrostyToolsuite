using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponUnlockPickupEntityData))]
	public class WeaponUnlockPickupEntity : PickupEntity, IEntityData<FrostySdk.Ebx.WeaponUnlockPickupEntityData>
	{
		public new FrostySdk.Ebx.WeaponUnlockPickupEntityData Data => data as FrostySdk.Ebx.WeaponUnlockPickupEntityData;

		public WeaponUnlockPickupEntity(FrostySdk.Ebx.WeaponUnlockPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

