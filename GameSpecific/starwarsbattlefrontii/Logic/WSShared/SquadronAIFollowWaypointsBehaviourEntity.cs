using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIFollowWaypointsBehaviourEntityData))]
	public class SquadronAIFollowWaypointsBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIFollowWaypointsBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIFollowWaypointsBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIFollowWaypointsBehaviourEntityData;
		public override string DisplayName => "SquadronAIFollowWaypointsBehaviour";

		public SquadronAIFollowWaypointsBehaviourEntity(FrostySdk.Ebx.SquadronAIFollowWaypointsBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

