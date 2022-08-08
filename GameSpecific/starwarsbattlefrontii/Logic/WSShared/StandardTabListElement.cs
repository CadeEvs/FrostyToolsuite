using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StandardTabListElementData))]
	public class StandardTabListElement : TabListElement, IEntityData<FrostySdk.Ebx.StandardTabListElementData>
	{
		public new FrostySdk.Ebx.StandardTabListElementData Data => data as FrostySdk.Ebx.StandardTabListElementData;
		public override string DisplayName => "StandardTabListElement";

		public StandardTabListElement(FrostySdk.Ebx.StandardTabListElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

