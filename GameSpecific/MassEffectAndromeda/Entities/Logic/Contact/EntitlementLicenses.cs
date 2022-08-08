using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntitlementLicensesData))]
	public class EntitlementLicenses : LogicEntity, IEntityData<FrostySdk.Ebx.EntitlementLicensesData>
	{
		public new FrostySdk.Ebx.EntitlementLicensesData Data => data as FrostySdk.Ebx.EntitlementLicensesData;
		public override string DisplayName => "EntitlementLicenses";

		public EntitlementLicenses(FrostySdk.Ebx.EntitlementLicensesData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

