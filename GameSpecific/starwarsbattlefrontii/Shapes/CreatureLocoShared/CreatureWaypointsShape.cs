using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureWaypointsShapeData))]
	public class CreatureWaypointsShape : WaypointsShape, IEntityData<FrostySdk.Ebx.CreatureWaypointsShapeData>
	{
		public new FrostySdk.Ebx.CreatureWaypointsShapeData Data => data as FrostySdk.Ebx.CreatureWaypointsShapeData;
		public override string DisplayName => "CreatureWaypointsShape";

		public CreatureWaypointsShape(FrostySdk.Ebx.CreatureWaypointsShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

