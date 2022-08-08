using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIWaypointsShapeData))]
	public class AIWaypointsShape : WaypointsShape, IEntityData<FrostySdk.Ebx.AIWaypointsShapeData>
	{
		public new FrostySdk.Ebx.AIWaypointsShapeData Data => data as FrostySdk.Ebx.AIWaypointsShapeData;
		public override string DisplayName => "AIWaypointsShape";

		public AIWaypointsShape(FrostySdk.Ebx.AIWaypointsShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

