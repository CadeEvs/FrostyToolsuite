using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticTabListCellWidgetData))]
	public class StaticTabListCellWidget : TabListCellWidget, IEntityData<FrostySdk.Ebx.StaticTabListCellWidgetData>
	{
		public new FrostySdk.Ebx.StaticTabListCellWidgetData Data => data as FrostySdk.Ebx.StaticTabListCellWidgetData;
		public override string DisplayName => "StaticTabListCellWidget";

		public StaticTabListCellWidget(FrostySdk.Ebx.StaticTabListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

