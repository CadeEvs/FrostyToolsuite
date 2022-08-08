using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverConversationCheckEntityData))]
	public class VoiceOverConversationCheckEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverConversationCheckEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverConversationCheckEntityData Data => data as FrostySdk.Ebx.VoiceOverConversationCheckEntityData;
		public override string DisplayName => "VoiceOverConversationCheck";

		public VoiceOverConversationCheckEntity(FrostySdk.Ebx.VoiceOverConversationCheckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

