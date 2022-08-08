using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotClearBooleanEntityData))]
	public class PlotClearBooleanEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotClearBooleanEntityData>
	{
		public new FrostySdk.Ebx.PlotClearBooleanEntityData Data => data as FrostySdk.Ebx.PlotClearBooleanEntityData;
		public override string DisplayName => "PlotClearBoolean";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Clear", Direction.In),
				new ConnectionDesc("OnClear", Direction.Out)
			};
		}

		public PlotClearBooleanEntity(FrostySdk.Ebx.PlotClearBooleanEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

