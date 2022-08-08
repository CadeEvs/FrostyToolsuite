using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsRagdollConstraintInitialStanceData))]
	public class PhysicsRagdollConstraintInitialStance : PhysicsConstraintInitialStance, IEntityData<FrostySdk.Ebx.PhysicsRagdollConstraintInitialStanceData>
	{
		public new FrostySdk.Ebx.PhysicsRagdollConstraintInitialStanceData Data => data as FrostySdk.Ebx.PhysicsRagdollConstraintInitialStanceData;
		public override string DisplayName => "PhysicsRagdollConstraintInitialStance";

		public PhysicsRagdollConstraintInitialStance(FrostySdk.Ebx.PhysicsRagdollConstraintInitialStanceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

