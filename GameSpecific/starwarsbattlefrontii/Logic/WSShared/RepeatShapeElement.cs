using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RepeatShapeElementData))]
	public class RepeatShapeElement : VectorShapeElement, IEntityData<FrostySdk.Ebx.RepeatShapeElementData>
	{
		public new FrostySdk.Ebx.RepeatShapeElementData Data => data as FrostySdk.Ebx.RepeatShapeElementData;
		public override string DisplayName => "RepeatShapeElement";

		public RepeatShapeElement(FrostySdk.Ebx.RepeatShapeElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

