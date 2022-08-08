using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CombatAreaTriggerEntityData))]
	public class CombatAreaTriggerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.CombatAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.CombatAreaTriggerEntityData Data => data as FrostySdk.Ebx.CombatAreaTriggerEntityData;

		public CombatAreaTriggerEntity(FrostySdk.Ebx.CombatAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

