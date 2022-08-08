using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UserPrivilegeCheckEntityData))]
	public class UserPrivilegeCheckEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UserPrivilegeCheckEntityData>
	{
		public new FrostySdk.Ebx.UserPrivilegeCheckEntityData Data => data as FrostySdk.Ebx.UserPrivilegeCheckEntityData;
		public override string DisplayName => "UserPrivilegeCheck";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UserPrivilegeCheckEntity(FrostySdk.Ebx.UserPrivilegeCheckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

