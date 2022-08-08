using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIRandomBehaviourEntityData))]
	public class SquadronAIRandomBehaviourEntity : SquadronAICompositeBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIRandomBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIRandomBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIRandomBehaviourEntityData;
		public override string DisplayName => "SquadronAIRandomBehaviour";

		public SquadronAIRandomBehaviourEntity(FrostySdk.Ebx.SquadronAIRandomBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

