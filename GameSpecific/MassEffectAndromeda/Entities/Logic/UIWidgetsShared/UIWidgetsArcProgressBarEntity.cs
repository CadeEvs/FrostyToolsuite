using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsArcProgressBarEntityData))]
	public class UIWidgetsArcProgressBarEntity : UIWidgetsRenderArcEntity, IEntityData<FrostySdk.Ebx.UIWidgetsArcProgressBarEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsArcProgressBarEntityData Data => data as FrostySdk.Ebx.UIWidgetsArcProgressBarEntityData;
		public override string DisplayName => "UIWidgetsArcProgressBar";

		public UIWidgetsArcProgressBarEntity(FrostySdk.Ebx.UIWidgetsArcProgressBarEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

