using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerEventEntityData))]
	public class TriggerEventEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.TriggerEventEntityData>
	{
		public new FrostySdk.Ebx.TriggerEventEntityData Data => data as FrostySdk.Ebx.TriggerEventEntityData;

		public TriggerEventEntity(FrostySdk.Ebx.TriggerEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

