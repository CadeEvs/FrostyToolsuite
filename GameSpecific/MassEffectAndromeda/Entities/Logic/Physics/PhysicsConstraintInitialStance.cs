using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsConstraintInitialStanceData))]
	public class PhysicsConstraintInitialStance : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsConstraintInitialStanceData>
	{
		public new FrostySdk.Ebx.PhysicsConstraintInitialStanceData Data => data as FrostySdk.Ebx.PhysicsConstraintInitialStanceData;
		public override string DisplayName => "PhysicsConstraintInitialStance";

		public PhysicsConstraintInitialStance(FrostySdk.Ebx.PhysicsConstraintInitialStanceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

