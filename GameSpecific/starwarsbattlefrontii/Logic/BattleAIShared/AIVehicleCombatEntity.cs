using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIVehicleCombatEntityData))]
	public class AIVehicleCombatEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIVehicleCombatEntityData>
	{
		public new FrostySdk.Ebx.AIVehicleCombatEntityData Data => data as FrostySdk.Ebx.AIVehicleCombatEntityData;
		public override string DisplayName => "AIVehicleCombat";

		public AIVehicleCombatEntity(FrostySdk.Ebx.AIVehicleCombatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

