using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAITurnBehaviourEntityData))]
	public class SquadronAITurnBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAITurnBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAITurnBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAITurnBehaviourEntityData;
		public override string DisplayName => "SquadronAITurnBehaviour";

		public SquadronAITurnBehaviourEntity(FrostySdk.Ebx.SquadronAITurnBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

