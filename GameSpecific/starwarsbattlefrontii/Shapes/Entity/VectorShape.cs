using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VectorShapeData))]
	public class VectorShape : BaseShape, IEntityData<FrostySdk.Ebx.VectorShapeData>
	{
		public new FrostySdk.Ebx.VectorShapeData Data => data as FrostySdk.Ebx.VectorShapeData;
		public override string DisplayName => "VectorShape";

		public VectorShape(FrostySdk.Ebx.VectorShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

