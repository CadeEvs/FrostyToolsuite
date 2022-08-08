using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsHingeConstraintInitialStanceData))]
	public class PhysicsHingeConstraintInitialStance : PhysicsConstraintInitialStance, IEntityData<FrostySdk.Ebx.PhysicsHingeConstraintInitialStanceData>
	{
		public new FrostySdk.Ebx.PhysicsHingeConstraintInitialStanceData Data => data as FrostySdk.Ebx.PhysicsHingeConstraintInitialStanceData;
		public override string DisplayName => "PhysicsHingeConstraintInitialStance";

		public PhysicsHingeConstraintInitialStance(FrostySdk.Ebx.PhysicsHingeConstraintInitialStanceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

