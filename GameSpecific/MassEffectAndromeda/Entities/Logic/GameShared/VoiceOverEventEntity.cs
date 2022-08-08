using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverEventEntityData))]
	public class VoiceOverEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverEventEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverEventEntityData Data => data as FrostySdk.Ebx.VoiceOverEventEntityData;
		public override string DisplayName => "VoiceOverEvent";

		public VoiceOverEventEntity(FrostySdk.Ebx.VoiceOverEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

