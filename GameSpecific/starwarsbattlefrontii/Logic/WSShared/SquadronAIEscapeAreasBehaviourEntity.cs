using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIEscapeAreasBehaviourEntityData))]
	public class SquadronAIEscapeAreasBehaviourEntity : SquadronAIBehaviourEntity, IEntityData<FrostySdk.Ebx.SquadronAIEscapeAreasBehaviourEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIEscapeAreasBehaviourEntityData Data => data as FrostySdk.Ebx.SquadronAIEscapeAreasBehaviourEntityData;
		public override string DisplayName => "SquadronAIEscapeAreasBehaviour";

		public SquadronAIEscapeAreasBehaviourEntity(FrostySdk.Ebx.SquadronAIEscapeAreasBehaviourEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

