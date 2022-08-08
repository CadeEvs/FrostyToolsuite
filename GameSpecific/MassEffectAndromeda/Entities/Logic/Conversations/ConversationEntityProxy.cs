using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationEntityProxyData))]
	public class ConversationEntityProxy : LogicEntity, IEntityData<FrostySdk.Ebx.ConversationEntityProxyData>
	{
		public new FrostySdk.Ebx.ConversationEntityProxyData Data => data as FrostySdk.Ebx.ConversationEntityProxyData;
		public override string DisplayName => "ConversationEntityProxy";

		public ConversationEntityProxy(FrostySdk.Ebx.ConversationEntityProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

