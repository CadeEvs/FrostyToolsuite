using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneVectorShapeData))]
	public class ZoneVectorShape : VectorShape, IEntityData<FrostySdk.Ebx.ZoneVectorShapeData>
	{
		public new FrostySdk.Ebx.ZoneVectorShapeData Data => data as FrostySdk.Ebx.ZoneVectorShapeData;
		public override string DisplayName => "ZoneVectorShape";

		public ZoneVectorShape(FrostySdk.Ebx.ZoneVectorShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

