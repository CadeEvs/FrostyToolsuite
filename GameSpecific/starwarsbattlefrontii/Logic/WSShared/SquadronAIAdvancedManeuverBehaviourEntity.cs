using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIAdvancedManeuverBehaviourEntityData))]
	public class SquadronAIAdvancedManeuverBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIAdvancedManeuverBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIAdvancedManeuverBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIAdvancedManeuverBehaviourEntityData;
		public override string DisplayName => "SquadronAIAdvancedManeuverBehaviour";

		public SquadronAIAdvancedManeuverBehaviourEntity(FrostySdk.Ebx.SquadronAIAdvancedManeuverBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

