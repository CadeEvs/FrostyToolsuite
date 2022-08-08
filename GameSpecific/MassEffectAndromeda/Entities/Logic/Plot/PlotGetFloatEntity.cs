using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotGetFloatEntityData))]
	public class PlotGetFloatEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotGetFloatEntityData>
	{
		public new FrostySdk.Ebx.PlotGetFloatEntityData Data => data as FrostySdk.Ebx.PlotGetFloatEntityData;
		public override string DisplayName => "PlotGetFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Get", Direction.In),
				new ConnectionDesc("OnGet", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out)
			};
		}

		public PlotGetFloatEntity(FrostySdk.Ebx.PlotGetFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

