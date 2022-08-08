using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LuaListElementData))]
	public class LuaListElement : ListElement, IEntityData<FrostySdk.Ebx.LuaListElementData>
	{
		public new FrostySdk.Ebx.LuaListElementData Data => data as FrostySdk.Ebx.LuaListElementData;
		public override string DisplayName => "LuaListElement";

		public LuaListElement(FrostySdk.Ebx.LuaListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

