using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotIncrementIntegerByPlotFlagValueEntityData))]
	public class PlotIncrementIntegerByPlotFlagValueEntity : PlotIncrementIntegerBaseEntity, IEntityData<FrostySdk.Ebx.PlotIncrementIntegerByPlotFlagValueEntityData>
	{
		public new FrostySdk.Ebx.PlotIncrementIntegerByPlotFlagValueEntityData Data => data as FrostySdk.Ebx.PlotIncrementIntegerByPlotFlagValueEntityData;
		public override string DisplayName => "PlotIncrementIntegerByPlotFlagValue";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlotIncrementIntegerByPlotFlagValueEntity(FrostySdk.Ebx.PlotIncrementIntegerByPlotFlagValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

