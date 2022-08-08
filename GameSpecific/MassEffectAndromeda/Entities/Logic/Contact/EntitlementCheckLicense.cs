using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntitlementCheckLicenseData))]
	public class EntitlementCheckLicense : LogicEntity, IEntityData<FrostySdk.Ebx.EntitlementCheckLicenseData>
	{
		public new FrostySdk.Ebx.EntitlementCheckLicenseData Data => data as FrostySdk.Ebx.EntitlementCheckLicenseData;
		public override string DisplayName => "EntitlementCheckLicense";

		public EntitlementCheckLicense(FrostySdk.Ebx.EntitlementCheckLicenseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

