using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotGetBooleanEntityData))]
	public class PlotGetBooleanEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotGetBooleanEntityData>
	{
		public new FrostySdk.Ebx.PlotGetBooleanEntityData Data => data as FrostySdk.Ebx.PlotGetBooleanEntityData;
		public override string DisplayName => "PlotGetBoolean";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Get", Direction.In),
				new ConnectionDesc("OnTrue", Direction.Out),
				new ConnectionDesc("OnFalse", Direction.Out)
			};
		}

        public PlotGetBooleanEntity(FrostySdk.Ebx.PlotGetBooleanEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

