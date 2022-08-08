using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsBallAndSocketConstraintData))]
	public class PhysicsBallAndSocketConstraint : PhysicsConstraint, IEntityData<FrostySdk.Ebx.PhysicsBallAndSocketConstraintData>
	{
		public new FrostySdk.Ebx.PhysicsBallAndSocketConstraintData Data => data as FrostySdk.Ebx.PhysicsBallAndSocketConstraintData;
		public override string DisplayName => "PhysicsBallAndSocketConstraint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsBallAndSocketConstraint(FrostySdk.Ebx.PhysicsBallAndSocketConstraintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

