using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerWeaponSyncEntityData))]
	public class TriggerWeaponSyncEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TriggerWeaponSyncEntityData>
	{
		public new FrostySdk.Ebx.TriggerWeaponSyncEntityData Data => data as FrostySdk.Ebx.TriggerWeaponSyncEntityData;
		public override string DisplayName => "TriggerWeaponSync";

		public TriggerWeaponSyncEntity(FrostySdk.Ebx.TriggerWeaponSyncEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

