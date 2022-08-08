using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAIChannelReferenceEntityData))]
	public class SquadronAIChannelReferenceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadronAIChannelReferenceEntityData>
	{
		public new FrostySdk.Ebx.SquadronAIChannelReferenceEntityData Data => data as FrostySdk.Ebx.SquadronAIChannelReferenceEntityData;
		public override string DisplayName => "SquadronAIChannelReference";

		public SquadronAIChannelReferenceEntity(FrostySdk.Ebx.SquadronAIChannelReferenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

