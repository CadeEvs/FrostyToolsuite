using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIBehaviourEntityData))]
	public class SquadronAIBehaviourEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIBehaviourEntityData;
		public override string DisplayName => "SquadronAIBehaviour";

		public SquadronAIBehaviourEntity(FrostySdk.Ebx.SquadronAIBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

