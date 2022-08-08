using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualVectorShapeData))]
	public class VisualVectorShape : VectorShape, IEntityData<FrostySdk.Ebx.VisualVectorShapeData>
	{
		public new FrostySdk.Ebx.VisualVectorShapeData Data => data as FrostySdk.Ebx.VisualVectorShapeData;
		public override string DisplayName => "VisualVectorShape";

		public VisualVectorShape(FrostySdk.Ebx.VisualVectorShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

