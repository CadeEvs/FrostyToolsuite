using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WebViewWidgetData))]
	public class WebViewWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.WebViewWidgetData>
	{
		public new FrostySdk.Ebx.WebViewWidgetData Data => data as FrostySdk.Ebx.WebViewWidgetData;
		public override string DisplayName => "WebViewWidget";

		public WebViewWidget(FrostySdk.Ebx.WebViewWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

