using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationInterruptionTriggerData))]
	public class ConversationInterruptionTrigger : LogicEntity, IEntityData<FrostySdk.Ebx.ConversationInterruptionTriggerData>
	{
		public new FrostySdk.Ebx.ConversationInterruptionTriggerData Data => data as FrostySdk.Ebx.ConversationInterruptionTriggerData;
		public override string DisplayName => "ConversationInterruptionTrigger";

		public ConversationInterruptionTrigger(FrostySdk.Ebx.ConversationInterruptionTriggerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

