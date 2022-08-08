using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleMESoldierCollisionComponentData))]
	public class VehicleMESoldierCollisionComponent : GameComponent, IEntityData<FrostySdk.Ebx.VehicleMESoldierCollisionComponentData>
	{
		public new FrostySdk.Ebx.VehicleMESoldierCollisionComponentData Data => data as FrostySdk.Ebx.VehicleMESoldierCollisionComponentData;
		public override string DisplayName => "VehicleMESoldierCollisionComponent";

		public VehicleMESoldierCollisionComponent(FrostySdk.Ebx.VehicleMESoldierCollisionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

