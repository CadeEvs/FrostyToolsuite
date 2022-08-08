using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsBallAndSocketConstraintInitialStanceData))]
	public class PhysicsBallAndSocketConstraintInitialStance : PhysicsConstraintInitialStance, IEntityData<FrostySdk.Ebx.PhysicsBallAndSocketConstraintInitialStanceData>
	{
		public new FrostySdk.Ebx.PhysicsBallAndSocketConstraintInitialStanceData Data => data as FrostySdk.Ebx.PhysicsBallAndSocketConstraintInitialStanceData;
		public override string DisplayName => "PhysicsBallAndSocketConstraintInitialStance";

		public PhysicsBallAndSocketConstraintInitialStance(FrostySdk.Ebx.PhysicsBallAndSocketConstraintInitialStanceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

