using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIDogfightingAttackBehaviourEntityData))]
	public class SquadronAIDogfightingAttackBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIDogfightingAttackBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIDogfightingAttackBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIDogfightingAttackBehaviourEntityData;
		public override string DisplayName => "SquadronAIDogfightingAttackBehaviour";

		public SquadronAIDogfightingAttackBehaviourEntity(FrostySdk.Ebx.SquadronAIDogfightingAttackBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

