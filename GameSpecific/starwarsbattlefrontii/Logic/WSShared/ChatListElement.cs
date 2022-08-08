using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChatListElementData))]
	public class ChatListElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ChatListElementData>
	{
		public new FrostySdk.Ebx.ChatListElementData Data => data as FrostySdk.Ebx.ChatListElementData;
		public override string DisplayName => "ChatListElement";

		public ChatListElement(FrostySdk.Ebx.ChatListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

