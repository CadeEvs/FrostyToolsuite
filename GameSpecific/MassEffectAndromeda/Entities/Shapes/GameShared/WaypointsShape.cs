using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaypointsShapeData))]
	public class WaypointsShape : VectorShape, IEntityData<FrostySdk.Ebx.WaypointsShapeData>
	{
		public new FrostySdk.Ebx.WaypointsShapeData Data => data as FrostySdk.Ebx.WaypointsShapeData;
		public override string DisplayName => "WaypointsShape";

		public WaypointsShape(FrostySdk.Ebx.WaypointsShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

