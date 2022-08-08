using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LuaCellWidgetData))]
	public class LuaCellWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.LuaCellWidgetData>
	{
		public new FrostySdk.Ebx.LuaCellWidgetData Data => data as FrostySdk.Ebx.LuaCellWidgetData;
		public override string DisplayName => "LuaCellWidget";

		public LuaCellWidget(FrostySdk.Ebx.LuaCellWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

