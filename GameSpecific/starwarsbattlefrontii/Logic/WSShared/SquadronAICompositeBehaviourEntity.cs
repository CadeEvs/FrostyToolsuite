using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAICompositeBehaviourEntityData))]
	public class SquadronAICompositeBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAICompositeBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAICompositeBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAICompositeBehaviourEntityData;
		public override string DisplayName => "SquadronAICompositeBehaviour";

		public SquadronAICompositeBehaviourEntity(FrostySdk.Ebx.SquadronAICompositeBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

