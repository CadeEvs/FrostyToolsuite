using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsConstraintMotorData))]
	public class PhysicsConstraintMotor : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsConstraintMotorData>
	{
		public new FrostySdk.Ebx.PhysicsConstraintMotorData Data => data as FrostySdk.Ebx.PhysicsConstraintMotorData;
		public override string DisplayName => "PhysicsConstraintMotor";

		public PhysicsConstraintMotor(FrostySdk.Ebx.PhysicsConstraintMotorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

