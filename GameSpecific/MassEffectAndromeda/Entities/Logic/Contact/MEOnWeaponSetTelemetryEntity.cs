using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEOnWeaponSetTelemetryEntityData))]
	public class MEOnWeaponSetTelemetryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEOnWeaponSetTelemetryEntityData>
	{
		public new FrostySdk.Ebx.MEOnWeaponSetTelemetryEntityData Data => data as FrostySdk.Ebx.MEOnWeaponSetTelemetryEntityData;
		public override string DisplayName => "MEOnWeaponSetTelemetry";

		public MEOnWeaponSetTelemetryEntity(FrostySdk.Ebx.MEOnWeaponSetTelemetryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

