using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SenseCombatEventEntityData))]
	public class SenseCombatEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SenseCombatEventEntityData>
	{
		public new FrostySdk.Ebx.SenseCombatEventEntityData Data => data as FrostySdk.Ebx.SenseCombatEventEntityData;
		public override string DisplayName => "SenseCombatEvent";

		public SenseCombatEventEntity(FrostySdk.Ebx.SenseCombatEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

