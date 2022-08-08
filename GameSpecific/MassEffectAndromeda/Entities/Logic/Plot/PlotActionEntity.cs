using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotActionEntityData))]
	public class PlotActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlotActionEntityData>
	{
		public new FrostySdk.Ebx.PlotActionEntityData Data => data as FrostySdk.Ebx.PlotActionEntityData;
		public override string DisplayName => "PlotAction";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Execute", Direction.In),
				new ConnectionDesc("OnExecute", Direction.Out)
			};
		}

		public PlotActionEntity(FrostySdk.Ebx.PlotActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

