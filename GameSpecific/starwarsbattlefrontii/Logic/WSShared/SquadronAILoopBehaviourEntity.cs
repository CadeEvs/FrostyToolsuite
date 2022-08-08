using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAILoopBehaviourEntityData))]
	public class SquadronAILoopBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAILoopBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAILoopBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAILoopBehaviourEntityData;
		public override string DisplayName => "SquadronAILoopBehaviour";

		public SquadronAILoopBehaviourEntity(FrostySdk.Ebx.SquadronAILoopBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

