using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsRootControlConstraintData))]
	public class PhysicsRootControlConstraint : PhysicsConstraint, IEntityData<FrostySdk.Ebx.PhysicsRootControlConstraintData>
	{
		public new FrostySdk.Ebx.PhysicsRootControlConstraintData Data => data as FrostySdk.Ebx.PhysicsRootControlConstraintData;
		public override string DisplayName => "PhysicsRootControlConstraint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsRootControlConstraint(FrostySdk.Ebx.PhysicsRootControlConstraintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

