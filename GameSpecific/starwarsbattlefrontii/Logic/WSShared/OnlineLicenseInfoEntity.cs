using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineLicenseInfoEntityData))]
	public class OnlineLicenseInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineLicenseInfoEntityData>
	{
		public new FrostySdk.Ebx.OnlineLicenseInfoEntityData Data => data as FrostySdk.Ebx.OnlineLicenseInfoEntityData;
		public override string DisplayName => "OnlineLicenseInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OnlineLicenseInfoEntity(FrostySdk.Ebx.OnlineLicenseInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

