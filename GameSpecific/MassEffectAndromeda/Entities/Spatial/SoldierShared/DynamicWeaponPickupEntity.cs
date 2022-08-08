using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicWeaponPickupEntityData))]
	public class DynamicWeaponPickupEntity : PickupEntity, IEntityData<FrostySdk.Ebx.DynamicWeaponPickupEntityData>
	{
		public new FrostySdk.Ebx.DynamicWeaponPickupEntityData Data => data as FrostySdk.Ebx.DynamicWeaponPickupEntityData;

		public DynamicWeaponPickupEntity(FrostySdk.Ebx.DynamicWeaponPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

