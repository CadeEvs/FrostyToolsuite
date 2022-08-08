using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsRootControlConstraintInitialStanceData))]
	public class PhysicsRootControlConstraintInitialStance : PhysicsConstraintInitialStance, IEntityData<FrostySdk.Ebx.PhysicsRootControlConstraintInitialStanceData>
	{
		public new FrostySdk.Ebx.PhysicsRootControlConstraintInitialStanceData Data => data as FrostySdk.Ebx.PhysicsRootControlConstraintInitialStanceData;
		public override string DisplayName => "PhysicsRootControlConstraintInitialStance";

		public PhysicsRootControlConstraintInitialStance(FrostySdk.Ebx.PhysicsRootControlConstraintInitialStanceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

