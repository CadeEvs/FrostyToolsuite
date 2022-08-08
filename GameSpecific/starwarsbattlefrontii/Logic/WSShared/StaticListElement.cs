using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticListElementData))]
	public class StaticListElement : ListElement, IEntityData<FrostySdk.Ebx.StaticListElementData>
	{
		public new FrostySdk.Ebx.StaticListElementData Data => data as FrostySdk.Ebx.StaticListElementData;
		public override string DisplayName => "StaticListElement";

		public StaticListElement(FrostySdk.Ebx.StaticListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

