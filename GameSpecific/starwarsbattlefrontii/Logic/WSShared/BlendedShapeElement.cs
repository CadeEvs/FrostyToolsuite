using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlendedShapeElementData))]
	public class BlendedShapeElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.BlendedShapeElementData>
	{
		public new FrostySdk.Ebx.BlendedShapeElementData Data => data as FrostySdk.Ebx.BlendedShapeElementData;
		public override string DisplayName => "BlendedShapeElement";

		public BlendedShapeElement(FrostySdk.Ebx.BlendedShapeElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

