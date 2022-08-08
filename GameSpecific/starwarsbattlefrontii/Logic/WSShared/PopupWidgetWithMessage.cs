using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PopupWidgetWithMessageData))]
	public class PopupWidgetWithMessage : PopupWidget, IEntityData<FrostySdk.Ebx.PopupWidgetWithMessageData>
	{
		public new FrostySdk.Ebx.PopupWidgetWithMessageData Data => data as FrostySdk.Ebx.PopupWidgetWithMessageData;
		public override string DisplayName => "PopupWidgetWithMessage";

		public PopupWidgetWithMessage(FrostySdk.Ebx.PopupWidgetWithMessageData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

