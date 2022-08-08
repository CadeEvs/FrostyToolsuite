using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MaskingFillElementData))]
	public class MaskingFillElement : WSUISoloBatchableElement, IEntityData<FrostySdk.Ebx.MaskingFillElementData>
	{
		public new FrostySdk.Ebx.MaskingFillElementData Data => data as FrostySdk.Ebx.MaskingFillElementData;
		public override string DisplayName => "MaskingFillElement";

		public MaskingFillElement(FrostySdk.Ebx.MaskingFillElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

