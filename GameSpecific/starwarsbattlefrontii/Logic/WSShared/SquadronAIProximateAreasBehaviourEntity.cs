using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIProximateAreasBehaviourEntityData))]
	public class SquadronAIProximateAreasBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIProximateAreasBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIProximateAreasBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIProximateAreasBehaviourEntityData;
		public override string DisplayName => "SquadronAIProximateAreasBehaviour";

		public SquadronAIProximateAreasBehaviourEntity(FrostySdk.Ebx.SquadronAIProximateAreasBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

