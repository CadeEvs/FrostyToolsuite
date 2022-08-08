using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ListElementData))]
	public class ListElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ListElementData>
	{
		public new FrostySdk.Ebx.ListElementData Data => data as FrostySdk.Ebx.ListElementData;
		public override string DisplayName => "ListElement";

		public ListElement(FrostySdk.Ebx.ListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

