using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIWeaponSlotOverrideEntityData))]
	public class AIWeaponSlotOverrideEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIWeaponSlotOverrideEntityData>
	{
		public new FrostySdk.Ebx.AIWeaponSlotOverrideEntityData Data => data as FrostySdk.Ebx.AIWeaponSlotOverrideEntityData;
		public override string DisplayName => "AIWeaponSlotOverride";

		public AIWeaponSlotOverrideEntity(FrostySdk.Ebx.AIWeaponSlotOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

