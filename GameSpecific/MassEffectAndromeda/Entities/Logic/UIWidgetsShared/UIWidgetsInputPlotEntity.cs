using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsInputPlotEntityData))]
	public class UIWidgetsInputPlotEntity : UIWidgetsElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsInputPlotEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsInputPlotEntityData Data => data as FrostySdk.Ebx.UIWidgetsInputPlotEntityData;
		public override string DisplayName => "UIWidgetsInputPlot";

		public UIWidgetsInputPlotEntity(FrostySdk.Ebx.UIWidgetsInputPlotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

