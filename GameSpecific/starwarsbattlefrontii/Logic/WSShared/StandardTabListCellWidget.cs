using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StandardTabListCellWidgetData))]
	public class StandardTabListCellWidget : TabListCellWidget, IEntityData<FrostySdk.Ebx.StandardTabListCellWidgetData>
	{
		public new FrostySdk.Ebx.StandardTabListCellWidgetData Data => data as FrostySdk.Ebx.StandardTabListCellWidgetData;
		public override string DisplayName => "StandardTabListCellWidget";

		public StandardTabListCellWidget(FrostySdk.Ebx.StandardTabListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

