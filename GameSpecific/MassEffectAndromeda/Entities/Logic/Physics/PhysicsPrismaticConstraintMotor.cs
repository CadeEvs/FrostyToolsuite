using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsPrismaticConstraintMotorData))]
	public class PhysicsPrismaticConstraintMotor : PhysicsConstraintMotor, IEntityData<FrostySdk.Ebx.PhysicsPrismaticConstraintMotorData>
	{
		public new FrostySdk.Ebx.PhysicsPrismaticConstraintMotorData Data => data as FrostySdk.Ebx.PhysicsPrismaticConstraintMotorData;
		public override string DisplayName => "PhysicsPrismaticConstraintMotor";

		public PhysicsPrismaticConstraintMotor(FrostySdk.Ebx.PhysicsPrismaticConstraintMotorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

