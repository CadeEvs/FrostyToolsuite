using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UITriangleSelectorWidgetData))]
	public class UITriangleSelectorWidget : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UITriangleSelectorWidgetData>
	{
		public new FrostySdk.Ebx.UITriangleSelectorWidgetData Data => data as FrostySdk.Ebx.UITriangleSelectorWidgetData;
		public override string DisplayName => "UITriangleSelectorWidget";

		public UITriangleSelectorWidget(FrostySdk.Ebx.UITriangleSelectorWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

