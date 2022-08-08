using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotIncrementIntegerBaseEntityData))]
	public class PlotIncrementIntegerBaseEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotIncrementIntegerBaseEntityData>
	{
		public new FrostySdk.Ebx.PlotIncrementIntegerBaseEntityData Data => data as FrostySdk.Ebx.PlotIncrementIntegerBaseEntityData;
		public override string DisplayName => "PlotIncrementIntegerBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlotIncrementIntegerBaseEntity(FrostySdk.Ebx.PlotIncrementIntegerBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

