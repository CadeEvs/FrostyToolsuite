using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIBasicManeuverBehaviourEntityData))]
	public class SquadronAIBasicManeuverBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIBasicManeuverBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIBasicManeuverBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIBasicManeuverBehaviourEntityData;
		public override string DisplayName => "SquadronAIBasicManeuverBehaviour";

		public SquadronAIBasicManeuverBehaviourEntity(FrostySdk.Ebx.SquadronAIBasicManeuverBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

