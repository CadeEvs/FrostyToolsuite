using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAISelectorBehaviourEntityData))]
	public class SquadronAISelectorBehaviourEntity : SquadronAICompositeBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAISelectorBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAISelectorBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAISelectorBehaviourEntityData;
		public override string DisplayName => "SquadronAISelectorBehaviour";

		public SquadronAISelectorBehaviourEntity(FrostySdk.Ebx.SquadronAISelectorBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

