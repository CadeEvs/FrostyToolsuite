using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameEventsListCellWidgetData))]
	public class GameEventsListCellWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.GameEventsListCellWidgetData>
	{
		public new FrostySdk.Ebx.GameEventsListCellWidgetData Data => data as FrostySdk.Ebx.GameEventsListCellWidgetData;
		public override string DisplayName => "GameEventsListCellWidget";

		public GameEventsListCellWidget(FrostySdk.Ebx.GameEventsListCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

