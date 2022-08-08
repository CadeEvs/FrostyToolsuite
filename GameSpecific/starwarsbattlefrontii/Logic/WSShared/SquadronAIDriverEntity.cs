using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIDriverEntityData))]
	public class SquadronAIDriverEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIDriverEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIDriverEntityData Data => data as FrostySdk.Ebx.SquadronAIDriverEntityData;
		public override string DisplayName => "SquadronAIDriver";

		public SquadronAIDriverEntity(FrostySdk.Ebx.SquadronAIDriverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

