using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MessageEntityData))]
	public class MessageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MessageEntityData>
	{
		public new FrostySdk.Ebx.MessageEntityData Data => data as FrostySdk.Ebx.MessageEntityData;
		public override string DisplayName => "Message";

		public MessageEntity(FrostySdk.Ebx.MessageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

