using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LuaWidgetData))]
	public class LuaWidget : WSUIWidgetEntity, IEntityData<FrostySdk.Ebx.LuaWidgetData>
	{
		public new FrostySdk.Ebx.LuaWidgetData Data => data as FrostySdk.Ebx.LuaWidgetData;
		public override string DisplayName => "LuaWidget";

		public LuaWidget(FrostySdk.Ebx.LuaWidgetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

