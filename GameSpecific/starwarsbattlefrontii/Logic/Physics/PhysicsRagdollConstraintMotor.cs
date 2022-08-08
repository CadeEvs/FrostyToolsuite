using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsRagdollConstraintMotorData))]
	public class PhysicsRagdollConstraintMotor : PhysicsConstraintMotor, IEntityData<FrostySdk.Ebx.PhysicsRagdollConstraintMotorData>
	{
		public new FrostySdk.Ebx.PhysicsRagdollConstraintMotorData Data => data as FrostySdk.Ebx.PhysicsRagdollConstraintMotorData;
		public override string DisplayName => "PhysicsRagdollConstraintMotor";

		public PhysicsRagdollConstraintMotor(FrostySdk.Ebx.PhysicsRagdollConstraintMotorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

