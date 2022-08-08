using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ListCellWidgetBaseData))]
	public class ListCellWidgetBase : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.ListCellWidgetBaseData>
	{
		public new FrostySdk.Ebx.ListCellWidgetBaseData Data => data as FrostySdk.Ebx.ListCellWidgetBaseData;
		public override string DisplayName => "ListCellWidgetBase";

		public ListCellWidgetBase(FrostySdk.Ebx.ListCellWidgetBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

