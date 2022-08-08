using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VectorShapeElementData))]
	public class VectorShapeElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.VectorShapeElementData>
	{
		public new FrostySdk.Ebx.VectorShapeElementData Data => data as FrostySdk.Ebx.VectorShapeElementData;
		public override string DisplayName => "VectorShapeElement";

		public VectorShapeElement(FrostySdk.Ebx.VectorShapeElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

