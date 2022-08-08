using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TabListElementData))]
	public class TabListElement : ListElement, IEntityData<FrostySdk.Ebx.TabListElementData>
	{
		public new FrostySdk.Ebx.TabListElementData Data => data as FrostySdk.Ebx.TabListElementData;
		public override string DisplayName => "TabListElement";

		public TabListElement(FrostySdk.Ebx.TabListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

