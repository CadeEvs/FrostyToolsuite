using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StandardListCellWidgetData))]
	public class StandardListCellWidget : ListCellWidget, IEntityData<FrostySdk.Ebx.StandardListCellWidgetData>
	{
		public new FrostySdk.Ebx.StandardListCellWidgetData Data => data as FrostySdk.Ebx.StandardListCellWidgetData;
		public override string DisplayName => "StandardListCellWidget";

		public StandardListCellWidget(FrostySdk.Ebx.StandardListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

