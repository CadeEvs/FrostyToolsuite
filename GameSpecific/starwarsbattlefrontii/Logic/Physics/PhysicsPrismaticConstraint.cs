using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsPrismaticConstraintData))]
	public class PhysicsPrismaticConstraint : PhysicsConstraint, IEntityData<FrostySdk.Ebx.PhysicsPrismaticConstraintData>
	{
		public new FrostySdk.Ebx.PhysicsPrismaticConstraintData Data => data as FrostySdk.Ebx.PhysicsPrismaticConstraintData;
		public override string DisplayName => "PhysicsPrismaticConstraint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsPrismaticConstraint(FrostySdk.Ebx.PhysicsPrismaticConstraintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

