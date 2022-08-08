using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsConstraintData))]
	public class PhysicsConstraint : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsConstraintData>
	{
		public new FrostySdk.Ebx.PhysicsConstraintData Data => data as FrostySdk.Ebx.PhysicsConstraintData;
		public override string DisplayName => "PhysicsConstraint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsConstraint(FrostySdk.Ebx.PhysicsConstraintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

