using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronVoiceOverTriggerEntityData))]
	public class SquadronVoiceOverTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronVoiceOverTriggerEntityData>
	{
		public new FrostySdk.Ebx.SquadronVoiceOverTriggerEntityData Data => data as FrostySdk.Ebx.SquadronVoiceOverTriggerEntityData;
		public override string DisplayName => "SquadronVoiceOverTrigger";

		public SquadronVoiceOverTriggerEntity(FrostySdk.Ebx.SquadronVoiceOverTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

