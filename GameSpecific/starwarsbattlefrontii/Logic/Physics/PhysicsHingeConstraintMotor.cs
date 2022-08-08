using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsHingeConstraintMotorData))]
	public class PhysicsHingeConstraintMotor : PhysicsConstraintMotor, IEntityData<FrostySdk.Ebx.PhysicsHingeConstraintMotorData>
	{
		public new FrostySdk.Ebx.PhysicsHingeConstraintMotorData Data => data as FrostySdk.Ebx.PhysicsHingeConstraintMotorData;
		public override string DisplayName => "PhysicsHingeConstraintMotor";

		public PhysicsHingeConstraintMotor(FrostySdk.Ebx.PhysicsHingeConstraintMotorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

