using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAITacticalSpawnSquadronEntityData))]
	public class SquadronAITacticalSpawnSquadronEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAITacticalSpawnSquadronEntityData>
	{
		public new FrostySdk.Ebx.SquadronAITacticalSpawnSquadronEntityData Data => data as FrostySdk.Ebx.SquadronAITacticalSpawnSquadronEntityData;
		public override string DisplayName => "SquadronAITacticalSpawnSquadron";

		public SquadronAITacticalSpawnSquadronEntity(FrostySdk.Ebx.SquadronAITacticalSpawnSquadronEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

