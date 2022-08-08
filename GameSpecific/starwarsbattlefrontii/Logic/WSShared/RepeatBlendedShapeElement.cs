using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RepeatBlendedShapeElementData))]
	public class RepeatBlendedShapeElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RepeatBlendedShapeElementData>
	{
		public new FrostySdk.Ebx.RepeatBlendedShapeElementData Data => data as FrostySdk.Ebx.RepeatBlendedShapeElementData;
		public override string DisplayName => "RepeatBlendedShapeElement";

		public RepeatBlendedShapeElement(FrostySdk.Ebx.RepeatBlendedShapeElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

