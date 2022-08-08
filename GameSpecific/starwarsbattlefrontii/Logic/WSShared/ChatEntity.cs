using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChatEntityData))]
	public class ChatEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChatEntityData>
	{
		public new FrostySdk.Ebx.ChatEntityData Data => data as FrostySdk.Ebx.ChatEntityData;
		public override string DisplayName => "Chat";

		public ChatEntity(FrostySdk.Ebx.ChatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

