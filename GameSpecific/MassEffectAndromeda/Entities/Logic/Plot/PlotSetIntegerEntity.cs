using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotSetIntegerEntityData))]
	public class PlotSetIntegerEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotSetIntegerEntityData>
	{
		public new FrostySdk.Ebx.PlotSetIntegerEntityData Data => data as FrostySdk.Ebx.PlotSetIntegerEntityData;
		public override string DisplayName => "PlotSetInteger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Set", Direction.In),
				new ConnectionDesc("OnSet", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.In)
			};
		}

		public PlotSetIntegerEntity(FrostySdk.Ebx.PlotSetIntegerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

