using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehiclePhysicsActionData))]
	public class VehiclePhysicsAction : PhysicsAction, IEntityData<FrostySdk.Ebx.VehiclePhysicsActionData>
	{
		public new FrostySdk.Ebx.VehiclePhysicsActionData Data => data as FrostySdk.Ebx.VehiclePhysicsActionData;
		public override string DisplayName => "VehiclePhysicsAction";

		public VehiclePhysicsAction(FrostySdk.Ebx.VehiclePhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

