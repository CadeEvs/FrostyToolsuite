using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIWeaponOverrideEntityData))]
	public class AIWeaponOverrideEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIWeaponOverrideEntityData>
	{
		public new FrostySdk.Ebx.AIWeaponOverrideEntityData Data => data as FrostySdk.Ebx.AIWeaponOverrideEntityData;
		public override string DisplayName => "AIWeaponOverride";

		public AIWeaponOverrideEntity(FrostySdk.Ebx.AIWeaponOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

