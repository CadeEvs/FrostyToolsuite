using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ListCellWidgetData))]
	public class ListCellWidget : ListCellWidgetBase, IEntityData<FrostySdk.Ebx.ListCellWidgetData>
	{
		public new FrostySdk.Ebx.ListCellWidgetData Data => data as FrostySdk.Ebx.ListCellWidgetData;
		public override string DisplayName => "ListCellWidget";

		public ListCellWidget(FrostySdk.Ebx.ListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

