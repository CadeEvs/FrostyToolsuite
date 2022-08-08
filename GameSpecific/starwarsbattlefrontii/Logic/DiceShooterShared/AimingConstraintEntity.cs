using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimingConstraintEntityData))]
	public class AimingConstraintEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AimingConstraintEntityData>
	{
		public new FrostySdk.Ebx.AimingConstraintEntityData Data => data as FrostySdk.Ebx.AimingConstraintEntityData;
		public override string DisplayName => "AimingConstraint";

		public AimingConstraintEntity(FrostySdk.Ebx.AimingConstraintEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

