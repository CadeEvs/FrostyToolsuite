using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceLineEventEntityData))]
	public class VoiceLineEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceLineEventEntityData>
	{
		public new FrostySdk.Ebx.VoiceLineEventEntityData Data => data as FrostySdk.Ebx.VoiceLineEventEntityData;
		public override string DisplayName => "VoiceLineEvent";

		public VoiceLineEventEntity(FrostySdk.Ebx.VoiceLineEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

