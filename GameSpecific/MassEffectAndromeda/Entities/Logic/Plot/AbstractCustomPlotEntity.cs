using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AbstractCustomPlotEntityData))]
	public class AbstractCustomPlotEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AbstractCustomPlotEntityData>
	{
		public new FrostySdk.Ebx.AbstractCustomPlotEntityData Data => data as FrostySdk.Ebx.AbstractCustomPlotEntityData;
		public override string DisplayName => "AbstractCustomPlot";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AbstractCustomPlotEntity(FrostySdk.Ebx.AbstractCustomPlotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

