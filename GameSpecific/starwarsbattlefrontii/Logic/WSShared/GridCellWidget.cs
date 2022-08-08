using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GridCellWidgetData))]
	public class GridCellWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.GridCellWidgetData>
	{
		public new FrostySdk.Ebx.GridCellWidgetData Data => data as FrostySdk.Ebx.GridCellWidgetData;
		public override string DisplayName => "GridCellWidget";

		public GridCellWidget(FrostySdk.Ebx.GridCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

