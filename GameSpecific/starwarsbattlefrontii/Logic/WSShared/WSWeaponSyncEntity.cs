using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSWeaponSyncEntityData))]
	public class WSWeaponSyncEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSWeaponSyncEntityData>
	{
		public new FrostySdk.Ebx.WSWeaponSyncEntityData Data => data as FrostySdk.Ebx.WSWeaponSyncEntityData;
		public override string DisplayName => "WSWeaponSync";

		public WSWeaponSyncEntity(FrostySdk.Ebx.WSWeaponSyncEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

