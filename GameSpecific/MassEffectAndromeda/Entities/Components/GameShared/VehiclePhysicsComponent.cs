using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehiclePhysicsComponentData))]
	public class VehiclePhysicsComponent : ControllablePhysicsComponent, IEntityData<FrostySdk.Ebx.VehiclePhysicsComponentData>
	{
		public new FrostySdk.Ebx.VehiclePhysicsComponentData Data => data as FrostySdk.Ebx.VehiclePhysicsComponentData;
		public override string DisplayName => "VehiclePhysicsComponent";

		public VehiclePhysicsComponent(FrostySdk.Ebx.VehiclePhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

