using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverPilotEntityData))]
	public class VoiceOverPilotEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverPilotEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverPilotEntityData Data => data as FrostySdk.Ebx.VoiceOverPilotEntityData;
		public override string DisplayName => "VoiceOverPilot";

		public VoiceOverPilotEntity(FrostySdk.Ebx.VoiceOverPilotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

