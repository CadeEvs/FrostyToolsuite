using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotCompareFloatEntityData))]
	public class PlotCompareFloatEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotCompareFloatEntityData>
	{
		public new FrostySdk.Ebx.PlotCompareFloatEntityData Data => data as FrostySdk.Ebx.PlotCompareFloatEntityData;
		public override string DisplayName => "PlotCompareFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlotCompareFloatEntity(FrostySdk.Ebx.PlotCompareFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

