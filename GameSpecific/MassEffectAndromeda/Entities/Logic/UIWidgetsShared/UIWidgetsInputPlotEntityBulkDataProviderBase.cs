using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsInputPlotEntityBulkDataProviderBaseData))]
	public class UIWidgetsInputPlotEntityBulkDataProviderBase : LogicEntity, IEntityData<FrostySdk.Ebx.UIWidgetsInputPlotEntityBulkDataProviderBaseData>
	{
		public new FrostySdk.Ebx.UIWidgetsInputPlotEntityBulkDataProviderBaseData Data => data as FrostySdk.Ebx.UIWidgetsInputPlotEntityBulkDataProviderBaseData;
		public override string DisplayName => "UIWidgetsInputPlotEntityBulkDataProviderBase";

		public UIWidgetsInputPlotEntityBulkDataProviderBase(FrostySdk.Ebx.UIWidgetsInputPlotEntityBulkDataProviderBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

