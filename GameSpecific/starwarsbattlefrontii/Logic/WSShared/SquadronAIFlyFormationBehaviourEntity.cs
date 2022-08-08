using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIFlyFormationBehaviourEntityData))]
	public class SquadronAIFlyFormationBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIFlyFormationBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIFlyFormationBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIFlyFormationBehaviourEntityData;
		public override string DisplayName => "SquadronAIFlyFormationBehaviour";

		public SquadronAIFlyFormationBehaviourEntity(FrostySdk.Ebx.SquadronAIFlyFormationBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

