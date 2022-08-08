using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextElementData))]
	public class TextElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.TextElementData>
	{
		public new FrostySdk.Ebx.TextElementData Data => data as FrostySdk.Ebx.TextElementData;
		public override string DisplayName => "TextElement";

		public TextElement(FrostySdk.Ebx.TextElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

