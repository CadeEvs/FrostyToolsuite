using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverConversationGroupEntityData))]
	public class VoiceOverConversationGroupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverConversationGroupEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverConversationGroupEntityData Data => data as FrostySdk.Ebx.VoiceOverConversationGroupEntityData;
		public override string DisplayName => "VoiceOverConversationGroup";

		public VoiceOverConversationGroupEntity(FrostySdk.Ebx.VoiceOverConversationGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

