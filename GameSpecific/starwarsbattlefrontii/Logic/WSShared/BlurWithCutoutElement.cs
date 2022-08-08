using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlurWithCutoutElementData))]
	public class BlurWithCutoutElement : WSUISoloBatchableElement, IEntityData<FrostySdk.Ebx.BlurWithCutoutElementData>
	{
		public new FrostySdk.Ebx.BlurWithCutoutElementData Data => data as FrostySdk.Ebx.BlurWithCutoutElementData;
		public override string DisplayName => "BlurWithCutoutElement";

		public BlurWithCutoutElement(FrostySdk.Ebx.BlurWithCutoutElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

