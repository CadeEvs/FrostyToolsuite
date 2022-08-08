using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StatEventTriggerEntityData))]
	public class StatEventTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StatEventTriggerEntityData>
	{
		public new FrostySdk.Ebx.StatEventTriggerEntityData Data => data as FrostySdk.Ebx.StatEventTriggerEntityData;
		public override string DisplayName => "StatEventTrigger";

		public StatEventTriggerEntity(FrostySdk.Ebx.StatEventTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

