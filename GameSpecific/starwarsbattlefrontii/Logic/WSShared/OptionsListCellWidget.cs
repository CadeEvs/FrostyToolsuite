using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OptionsListCellWidgetData))]
	public class OptionsListCellWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.OptionsListCellWidgetData>
	{
		public new FrostySdk.Ebx.OptionsListCellWidgetData Data => data as FrostySdk.Ebx.OptionsListCellWidgetData;
		public override string DisplayName => "OptionsListCellWidget";

		public OptionsListCellWidget(FrostySdk.Ebx.OptionsListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

