using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RoundedFrameElementData))]
	public class RoundedFrameElement : WSUIBatchableElement, IEntityData<FrostySdk.Ebx.RoundedFrameElementData>
	{
		public new FrostySdk.Ebx.RoundedFrameElementData Data => data as FrostySdk.Ebx.RoundedFrameElementData;
		public override string DisplayName => "RoundedFrameElement";

		public RoundedFrameElement(FrostySdk.Ebx.RoundedFrameElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

