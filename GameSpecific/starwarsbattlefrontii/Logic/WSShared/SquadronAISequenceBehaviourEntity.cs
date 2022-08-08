using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAISequenceBehaviourEntityData))]
	public class SquadronAISequenceBehaviourEntity : SquadronAICompositeBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAISequenceBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAISequenceBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAISequenceBehaviourEntityData;
		public override string DisplayName => "SquadronAISequenceBehaviour";

		public SquadronAISequenceBehaviourEntity(FrostySdk.Ebx.SquadronAISequenceBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

