using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotGetIntegerEntityData))]
	public class PlotGetIntegerEntity : AbstractCustomPlotEntity, IEntityData<FrostySdk.Ebx.PlotGetIntegerEntityData>
	{
		public new FrostySdk.Ebx.PlotGetIntegerEntityData Data => data as FrostySdk.Ebx.PlotGetIntegerEntityData;
		public override string DisplayName => "PlotGetInteger";
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

        public PlotGetIntegerEntity(FrostySdk.Ebx.PlotGetIntegerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

