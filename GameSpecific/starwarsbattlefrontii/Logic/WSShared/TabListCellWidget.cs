using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TabListCellWidgetData))]
	public class TabListCellWidget : ListCellWidgetBase, IEntityData<FrostySdk.Ebx.TabListCellWidgetData>
	{
		public new FrostySdk.Ebx.TabListCellWidgetData Data => data as FrostySdk.Ebx.TabListCellWidgetData;
		public override string DisplayName => "TabListCellWidget";

		public TabListCellWidget(FrostySdk.Ebx.TabListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

