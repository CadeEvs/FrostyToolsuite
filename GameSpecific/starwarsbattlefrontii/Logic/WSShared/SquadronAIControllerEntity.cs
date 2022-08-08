using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIControllerEntityData))]
	public class SquadronAIControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIControllerEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIControllerEntityData Data => data as FrostySdk.Ebx.SquadronAIControllerEntityData;
		public override string DisplayName => "SquadronAIController";

		public SquadronAIControllerEntity(FrostySdk.Ebx.SquadronAIControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

