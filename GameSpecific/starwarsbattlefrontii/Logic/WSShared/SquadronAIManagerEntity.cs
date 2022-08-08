using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIManagerEntityData))]
	public class SquadronAIManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIManagerEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIManagerEntityData Data => data as FrostySdk.Ebx.SquadronAIManagerEntityData;
		public override string DisplayName => "SquadronAIManager";

		public SquadronAIManagerEntity(FrostySdk.Ebx.SquadronAIManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

