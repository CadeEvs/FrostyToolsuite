using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlurredBackgroundElementData))]
	public class BlurredBackgroundElement : WSUIBatchableElement, IEntityData<FrostySdk.Ebx.BlurredBackgroundElementData>
	{
		public new FrostySdk.Ebx.BlurredBackgroundElementData Data => data as FrostySdk.Ebx.BlurredBackgroundElementData;
		public override string DisplayName => "BlurredBackgroundElement";

		public BlurredBackgroundElement(FrostySdk.Ebx.BlurredBackgroundElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

