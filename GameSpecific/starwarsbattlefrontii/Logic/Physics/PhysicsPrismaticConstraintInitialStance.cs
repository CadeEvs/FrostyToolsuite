using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsPrismaticConstraintInitialStanceData))]
	public class PhysicsPrismaticConstraintInitialStance : PhysicsConstraintInitialStance, IEntityData<FrostySdk.Ebx.PhysicsPrismaticConstraintInitialStanceData>
	{
		public new FrostySdk.Ebx.PhysicsPrismaticConstraintInitialStanceData Data => data as FrostySdk.Ebx.PhysicsPrismaticConstraintInitialStanceData;
		public override string DisplayName => "PhysicsPrismaticConstraintInitialStance";

		public PhysicsPrismaticConstraintInitialStance(FrostySdk.Ebx.PhysicsPrismaticConstraintInitialStanceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

