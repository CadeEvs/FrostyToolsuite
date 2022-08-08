using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MultipleTriggerEntityData))]
	public class MultipleTriggerEntity : TriggerEventEntity, IEntityData<FrostySdk.Ebx.MultipleTriggerEntityData>
	{
		public new FrostySdk.Ebx.MultipleTriggerEntityData Data => data as FrostySdk.Ebx.MultipleTriggerEntityData;

		public MultipleTriggerEntity(FrostySdk.Ebx.MultipleTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

