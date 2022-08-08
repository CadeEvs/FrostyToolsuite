using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimingAngularSpeedConstraintDataProviderEntityData))]
	public class AimingAngularSpeedConstraintDataProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AimingAngularSpeedConstraintDataProviderEntityData>
	{
		public new FrostySdk.Ebx.AimingAngularSpeedConstraintDataProviderEntityData Data => data as FrostySdk.Ebx.AimingAngularSpeedConstraintDataProviderEntityData;
		public override string DisplayName => "AimingAngularSpeedConstraintDataProvider";

		public AimingAngularSpeedConstraintDataProviderEntity(FrostySdk.Ebx.AimingAngularSpeedConstraintDataProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

