using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAISideToSideBehaviourEntityData))]
	public class SquadronAISideToSideBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAISideToSideBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAISideToSideBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAISideToSideBehaviourEntityData;
		public override string DisplayName => "SquadronAISideToSideBehaviour";

		public SquadronAISideToSideBehaviourEntity(FrostySdk.Ebx.SquadronAISideToSideBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

