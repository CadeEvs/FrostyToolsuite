using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsConstraintOwnerData))]
	public class PhysicsConstraintOwner : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsConstraintOwnerData>
	{
		public new FrostySdk.Ebx.PhysicsConstraintOwnerData Data => data as FrostySdk.Ebx.PhysicsConstraintOwnerData;
		public override string DisplayName => "PhysicsConstraintOwner";

		public PhysicsConstraintOwner(FrostySdk.Ebx.PhysicsConstraintOwnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

