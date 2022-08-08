using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIProximateWaypointsBehaviourEntityData))]
	public class SquadronAIProximateWaypointsBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIProximateWaypointsBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIProximateWaypointsBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIProximateWaypointsBehaviourEntityData;
		public override string DisplayName => "SquadronAIProximateWaypointsBehaviour";

		public SquadronAIProximateWaypointsBehaviourEntity(FrostySdk.Ebx.SquadronAIProximateWaypointsBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

