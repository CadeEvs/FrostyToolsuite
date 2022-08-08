using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIDogfightingFlyForwardBehaviourEntityData))]
	public class SquadronAIDogfightingFlyForwardBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIDogfightingFlyForwardBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIDogfightingFlyForwardBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIDogfightingFlyForwardBehaviourEntityData;
		public override string DisplayName => "SquadronAIDogfightingFlyForwardBehaviour";

		public SquadronAIDogfightingFlyForwardBehaviourEntity(FrostySdk.Ebx.SquadronAIDogfightingFlyForwardBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

