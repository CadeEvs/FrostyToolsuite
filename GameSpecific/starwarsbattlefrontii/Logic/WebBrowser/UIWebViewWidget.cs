using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWebViewWidgetData))]
	public class UIWebViewWidget : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIWebViewWidgetData>
	{
		public new FrostySdk.Ebx.UIWebViewWidgetData Data => data as FrostySdk.Ebx.UIWebViewWidgetData;
		public override string DisplayName => "UIWebViewWidget";

		public UIWebViewWidget(FrostySdk.Ebx.UIWebViewWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

