using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIDogfightingEvadeBehaviourEntityData))]
	public class SquadronAIDogfightingEvadeBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIDogfightingEvadeBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIDogfightingEvadeBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIDogfightingEvadeBehaviourEntityData;
		public override string DisplayName => "SquadronAIDogfightingEvadeBehaviour";

		public SquadronAIDogfightingEvadeBehaviourEntity(FrostySdk.Ebx.SquadronAIDogfightingEvadeBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

