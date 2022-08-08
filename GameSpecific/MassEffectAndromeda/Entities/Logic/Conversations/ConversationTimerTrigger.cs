using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationTimerTriggerData))]
	public class ConversationTimerTrigger : LogicEntity, IEntityData<FrostySdk.Ebx.ConversationTimerTriggerData>
	{
		public new FrostySdk.Ebx.ConversationTimerTriggerData Data => data as FrostySdk.Ebx.ConversationTimerTriggerData;
		public override string DisplayName => "ConversationTimerTrigger";

		public ConversationTimerTrigger(FrostySdk.Ebx.ConversationTimerTriggerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

