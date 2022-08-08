using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsHingeConstraintData))]
	public class PhysicsHingeConstraint : PhysicsConstraint, IEntityData<FrostySdk.Ebx.PhysicsHingeConstraintData>
	{
		public new FrostySdk.Ebx.PhysicsHingeConstraintData Data => data as FrostySdk.Ebx.PhysicsHingeConstraintData;
		public override string DisplayName => "PhysicsHingeConstraint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsHingeConstraint(FrostySdk.Ebx.PhysicsHingeConstraintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

