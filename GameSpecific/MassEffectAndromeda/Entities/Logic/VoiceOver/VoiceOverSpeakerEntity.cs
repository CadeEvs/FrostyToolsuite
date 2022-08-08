using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverSpeakerEntityData))]
	public class VoiceOverSpeakerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverSpeakerEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverSpeakerEntityData Data => data as FrostySdk.Ebx.VoiceOverSpeakerEntityData;
		public override string DisplayName => "VoiceOverSpeaker";

		public VoiceOverSpeakerEntity(FrostySdk.Ebx.VoiceOverSpeakerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

