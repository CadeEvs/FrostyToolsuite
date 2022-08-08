using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContentLicenseInfoEntityData))]
	public class ContentLicenseInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ContentLicenseInfoEntityData>
	{
		public new FrostySdk.Ebx.ContentLicenseInfoEntityData Data => data as FrostySdk.Ebx.ContentLicenseInfoEntityData;
		public override string DisplayName => "ContentLicenseInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContentLicenseInfoEntity(FrostySdk.Ebx.ContentLicenseInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

