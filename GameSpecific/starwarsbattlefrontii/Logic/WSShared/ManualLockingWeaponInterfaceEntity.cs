using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ManualLockingWeaponInterfaceEntityData))]
	public class ManualLockingWeaponInterfaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ManualLockingWeaponInterfaceEntityData>
	{
		public new FrostySdk.Ebx.ManualLockingWeaponInterfaceEntityData Data => data as FrostySdk.Ebx.ManualLockingWeaponInterfaceEntityData;
		public override string DisplayName => "ManualLockingWeaponInterface";

		public ManualLockingWeaponInterfaceEntity(FrostySdk.Ebx.ManualLockingWeaponInterfaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

