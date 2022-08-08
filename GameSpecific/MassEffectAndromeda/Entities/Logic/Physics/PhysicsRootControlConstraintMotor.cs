using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsRootControlConstraintMotorData))]
	public class PhysicsRootControlConstraintMotor : PhysicsConstraintMotor, IEntityData<FrostySdk.Ebx.PhysicsRootControlConstraintMotorData>
	{
		public new FrostySdk.Ebx.PhysicsRootControlConstraintMotorData Data => data as FrostySdk.Ebx.PhysicsRootControlConstraintMotorData;
		public override string DisplayName => "PhysicsRootControlConstraintMotor";

		public PhysicsRootControlConstraintMotor(FrostySdk.Ebx.PhysicsRootControlConstraintMotorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

