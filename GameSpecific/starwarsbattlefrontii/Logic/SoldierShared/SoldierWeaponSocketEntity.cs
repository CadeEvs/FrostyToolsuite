using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierWeaponSocketEntityData))]
	public class SoldierWeaponSocketEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoldierWeaponSocketEntityData>
	{
		public new FrostySdk.Ebx.SoldierWeaponSocketEntityData Data => data as FrostySdk.Ebx.SoldierWeaponSocketEntityData;
		public override string DisplayName => "SoldierWeaponSocket";

		public SoldierWeaponSocketEntity(FrostySdk.Ebx.SoldierWeaponSocketEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

