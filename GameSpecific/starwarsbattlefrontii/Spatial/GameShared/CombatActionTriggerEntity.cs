using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CombatActionTriggerEntityData))]
	public class CombatActionTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.CombatActionTriggerEntityData>
	{
		public new FrostySdk.Ebx.CombatActionTriggerEntityData Data => data as FrostySdk.Ebx.CombatActionTriggerEntityData;

		public CombatActionTriggerEntity(FrostySdk.Ebx.CombatActionTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

