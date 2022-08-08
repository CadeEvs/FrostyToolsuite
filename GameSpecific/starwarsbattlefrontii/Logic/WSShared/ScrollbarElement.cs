using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScrollbarElementData))]
	public class ScrollbarElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ScrollbarElementData>
	{
		public new FrostySdk.Ebx.ScrollbarElementData Data => data as FrostySdk.Ebx.ScrollbarElementData;
		public override string DisplayName => "ScrollbarElement";

		public ScrollbarElement(FrostySdk.Ebx.ScrollbarElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

