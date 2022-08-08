using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEWaypointsShapeData))]
	public class MEWaypointsShape : WaypointsShape, IEntityData<FrostySdk.Ebx.MEWaypointsShapeData>
	{
		public new FrostySdk.Ebx.MEWaypointsShapeData Data => data as FrostySdk.Ebx.MEWaypointsShapeData;
		public override string DisplayName => "MEWaypointsShape";

		public MEWaypointsShape(FrostySdk.Ebx.MEWaypointsShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

