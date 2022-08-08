using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NavigationFrameWidgetData))]
	public class NavigationFrameWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.NavigationFrameWidgetData>
	{
		public new FrostySdk.Ebx.NavigationFrameWidgetData Data => data as FrostySdk.Ebx.NavigationFrameWidgetData;
		public override string DisplayName => "NavigationFrameWidget";

		public NavigationFrameWidget(FrostySdk.Ebx.NavigationFrameWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

