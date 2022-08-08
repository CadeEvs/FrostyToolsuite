using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageAreaTriggerEntityData))]
	public class DamageAreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.DamageAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.DamageAreaTriggerEntityData Data => data as FrostySdk.Ebx.DamageAreaTriggerEntityData;

		public DamageAreaTriggerEntity(FrostySdk.Ebx.DamageAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

