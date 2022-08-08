using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverConversationEntityData))]
	public class VoiceOverConversationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverConversationEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverConversationEntityData Data => data as FrostySdk.Ebx.VoiceOverConversationEntityData;
		public override string DisplayName => "VoiceOverConversation";

		public VoiceOverConversationEntity(FrostySdk.Ebx.VoiceOverConversationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

