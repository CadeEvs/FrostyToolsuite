using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PopupWidgetData))]
	public class PopupWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.PopupWidgetData>
	{
		public new FrostySdk.Ebx.PopupWidgetData Data => data as FrostySdk.Ebx.PopupWidgetData;
		public override string DisplayName => "PopupWidget";

		public PopupWidget(FrostySdk.Ebx.PopupWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

