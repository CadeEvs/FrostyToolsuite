using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProgressSvgVectorShapeElementData))]
	public class ProgressSvgVectorShapeElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ProgressSvgVectorShapeElementData>
	{
		public new FrostySdk.Ebx.ProgressSvgVectorShapeElementData Data => data as FrostySdk.Ebx.ProgressSvgVectorShapeElementData;
		public override string DisplayName => "ProgressSvgVectorShapeElement";

		public ProgressSvgVectorShapeElement(FrostySdk.Ebx.ProgressSvgVectorShapeElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

