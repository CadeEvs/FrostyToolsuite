using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierWeaponData))]
	public class SoldierWeapon : WeaponEntity, IEntityData<FrostySdk.Ebx.SoldierWeaponData>
	{
		public new FrostySdk.Ebx.SoldierWeaponData Data => data as FrostySdk.Ebx.SoldierWeaponData;

		public SoldierWeapon(FrostySdk.Ebx.SoldierWeaponData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

