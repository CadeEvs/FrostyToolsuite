using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StandardListElementData))]
	public class StandardListElement : ListElement, IEntityData<FrostySdk.Ebx.StandardListElementData>
	{
		public new FrostySdk.Ebx.StandardListElementData Data => data as FrostySdk.Ebx.StandardListElementData;
		public override string DisplayName => "StandardListElement";

		public StandardListElement(FrostySdk.Ebx.StandardListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

