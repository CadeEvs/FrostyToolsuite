using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotSetBooleanEntityData))]
	public class PlotSetBooleanEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotSetBooleanEntityData>
	{
		public new FrostySdk.Ebx.PlotSetBooleanEntityData Data => data as FrostySdk.Ebx.PlotSetBooleanEntityData;
		public override string DisplayName => "PlotSetBoolean";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Set", Direction.In),
				new ConnectionDesc("OnSet", Direction.Out)
			};
		}

		public PlotSetBooleanEntity(FrostySdk.Ebx.PlotSetBooleanEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

