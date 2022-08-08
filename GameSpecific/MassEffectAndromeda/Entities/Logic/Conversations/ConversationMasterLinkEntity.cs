using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationMasterLinkEntityData))]
	public class ConversationMasterLinkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConversationMasterLinkEntityData>
	{
		public new FrostySdk.Ebx.ConversationMasterLinkEntityData Data => data as FrostySdk.Ebx.ConversationMasterLinkEntityData;
		public override string DisplayName => "ConversationMasterLink";

		public ConversationMasterLinkEntity(FrostySdk.Ebx.ConversationMasterLinkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

