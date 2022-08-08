using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FrameInterpolatedTransformData))]
	public class FrameInterpolatedTransform : LogicEntity, IEntityData<FrostySdk.Ebx.FrameInterpolatedTransformData>
	{
		public new FrostySdk.Ebx.FrameInterpolatedTransformData Data => data as FrostySdk.Ebx.FrameInterpolatedTransformData;
		public override string DisplayName => "FrameInterpolatedTransform";

		public FrameInterpolatedTransform(FrostySdk.Ebx.FrameInterpolatedTransformData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

