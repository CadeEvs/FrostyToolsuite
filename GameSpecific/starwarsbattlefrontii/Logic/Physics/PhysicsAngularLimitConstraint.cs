using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsAngularLimitConstraintData))]
	public class PhysicsAngularLimitConstraint : PhysicsConstraint, IEntityData<FrostySdk.Ebx.PhysicsAngularLimitConstraintData>
	{
		public new FrostySdk.Ebx.PhysicsAngularLimitConstraintData Data => data as FrostySdk.Ebx.PhysicsAngularLimitConstraintData;
		public override string DisplayName => "PhysicsAngularLimitConstraint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsAngularLimitConstraint(FrostySdk.Ebx.PhysicsAngularLimitConstraintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

