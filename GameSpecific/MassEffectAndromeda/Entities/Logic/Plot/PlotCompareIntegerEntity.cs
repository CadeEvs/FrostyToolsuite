using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotCompareIntegerEntityData))]
	public class PlotCompareIntegerEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotCompareIntegerEntityData>
	{
		public new FrostySdk.Ebx.PlotCompareIntegerEntityData Data => data as FrostySdk.Ebx.PlotCompareIntegerEntityData;
		public override string DisplayName => "PlotCompareInteger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Get", Direction.In),
                new ConnectionDesc() { Name = "A>B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A>=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A!=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A<=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A<B", Direction = Direction.Out }
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("ValueA", Direction.Out),
                new ConnectionDesc("ValueB", Direction.Out)
            };
        }

        public PlotCompareIntegerEntity(FrostySdk.Ebx.PlotCompareIntegerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

