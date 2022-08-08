using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAITimedBehaviourEntityData))]
	public class SquadronAITimedBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAITimedBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAITimedBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAITimedBehaviourEntityData;
		public override string DisplayName => "SquadronAITimedBehaviour";

		public SquadronAITimedBehaviourEntity(FrostySdk.Ebx.SquadronAITimedBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

