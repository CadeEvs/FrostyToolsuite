using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticListCellWidgetData))]
	public class StaticListCellWidget : ListCellWidget, IEntityData<FrostySdk.Ebx.StaticListCellWidgetData>
	{
		public new FrostySdk.Ebx.StaticListCellWidgetData Data => data as FrostySdk.Ebx.StaticListCellWidgetData;
		public override string DisplayName => "StaticListCellWidget";

		public StaticListCellWidget(FrostySdk.Ebx.StaticListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

