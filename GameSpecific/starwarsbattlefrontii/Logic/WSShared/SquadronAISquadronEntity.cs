using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAISquadronEntityData))]
	public class SquadronAISquadronEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAISquadronEntityData>
	{
		public new FrostySdk.Ebx.SquadronAISquadronEntityData Data => data as FrostySdk.Ebx.SquadronAISquadronEntityData;
		public override string DisplayName => "SquadronAISquadron";

		public SquadronAISquadronEntity(FrostySdk.Ebx.SquadronAISquadronEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

