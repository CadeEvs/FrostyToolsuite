using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsRagdollConstraintData))]
	public class PhysicsRagdollConstraint : PhysicsConstraint, IEntityData<FrostySdk.Ebx.PhysicsRagdollConstraintData>
	{
		public new FrostySdk.Ebx.PhysicsRagdollConstraintData Data => data as FrostySdk.Ebx.PhysicsRagdollConstraintData;
		public override string DisplayName => "PhysicsRagdollConstraint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsRagdollConstraint(FrostySdk.Ebx.PhysicsRagdollConstraintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

