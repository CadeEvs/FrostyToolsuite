using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChatListCellWidgetData))]
	public class ChatListCellWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.ChatListCellWidgetData>
	{
		public new FrostySdk.Ebx.ChatListCellWidgetData Data => data as FrostySdk.Ebx.ChatListCellWidgetData;
		public override string DisplayName => "ChatListCellWidget";

		public ChatListCellWidget(FrostySdk.Ebx.ChatListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

