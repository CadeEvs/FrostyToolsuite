using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIChannelEntityData))]
	public class SquadronAIChannelEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIChannelEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIChannelEntityData Data => data as FrostySdk.Ebx.SquadronAIChannelEntityData;
		public override string DisplayName => "SquadronAIChannel";

		public SquadronAIChannelEntity(FrostySdk.Ebx.SquadronAIChannelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

