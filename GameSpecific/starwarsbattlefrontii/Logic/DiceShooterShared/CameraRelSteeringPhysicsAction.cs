using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraRelSteeringPhysicsActionData))]
	public class CameraRelSteeringPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.CameraRelSteeringPhysicsActionData>
	{
		public new FrostySdk.Ebx.CameraRelSteeringPhysicsActionData Data => data as FrostySdk.Ebx.CameraRelSteeringPhysicsActionData;
		public override string DisplayName => "CameraRelSteeringPhysicsAction";

		public CameraRelSteeringPhysicsAction(FrostySdk.Ebx.CameraRelSteeringPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

