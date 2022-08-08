using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAICollisionAvoidanceOverrideEntityData))]
	public class SquadronAICollisionAvoidanceOverrideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAICollisionAvoidanceOverrideEntityData>
	{
		public new FrostySdk.Ebx.SquadronAICollisionAvoidanceOverrideEntityData Data => data as FrostySdk.Ebx.SquadronAICollisionAvoidanceOverrideEntityData;
		public override string DisplayName => "SquadronAICollisionAvoidanceOverride";

		public SquadronAICollisionAvoidanceOverrideEntity(FrostySdk.Ebx.SquadronAICollisionAvoidanceOverrideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

