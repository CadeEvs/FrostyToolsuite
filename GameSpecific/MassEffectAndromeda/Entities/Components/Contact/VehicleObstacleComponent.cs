using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleObstacleComponentData))]
	public class VehicleObstacleComponent : GameComponent, IEntityData<FrostySdk.Ebx.VehicleObstacleComponentData>
	{
		public new FrostySdk.Ebx.VehicleObstacleComponentData Data => data as FrostySdk.Ebx.VehicleObstacleComponentData;
		public override string DisplayName => "VehicleObstacleComponent";

		public VehicleObstacleComponent(FrostySdk.Ebx.VehicleObstacleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

