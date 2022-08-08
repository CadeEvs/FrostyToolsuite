using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntitlementGrantLicenseData))]
	public class EntitlementGrantLicense : LogicEntity, IEntityData<FrostySdk.Ebx.EntitlementGrantLicenseData>
	{
		public new FrostySdk.Ebx.EntitlementGrantLicenseData Data => data as FrostySdk.Ebx.EntitlementGrantLicenseData;
		public override string DisplayName => "EntitlementGrantLicense";

		public EntitlementGrantLicense(FrostySdk.Ebx.EntitlementGrantLicenseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

