using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotSetFloatEntityData))]
	public class PlotSetFloatEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotSetFloatEntityData>
	{
		public new FrostySdk.Ebx.PlotSetFloatEntityData Data => data as FrostySdk.Ebx.PlotSetFloatEntityData;
		public override string DisplayName => "PlotSetFloat";
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

		public PlotSetFloatEntity(FrostySdk.Ebx.PlotSetFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

