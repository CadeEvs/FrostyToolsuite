using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DelayTriggerEntityData))]
	public class DelayTriggerEntity : TriggerEventEntity, IEntityData<FrostySdk.Ebx.DelayTriggerEntityData>
	{
		public new FrostySdk.Ebx.DelayTriggerEntityData Data => data as FrostySdk.Ebx.DelayTriggerEntityData;

		public DelayTriggerEntity(FrostySdk.Ebx.DelayTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

