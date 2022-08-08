using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextureElementData))]
	public class TextureElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.TextureElementData>
	{
		public new FrostySdk.Ebx.TextureElementData Data => data as FrostySdk.Ebx.TextureElementData;
		public override string DisplayName => "TextureElement";

		public TextureElement(FrostySdk.Ebx.TextureElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

