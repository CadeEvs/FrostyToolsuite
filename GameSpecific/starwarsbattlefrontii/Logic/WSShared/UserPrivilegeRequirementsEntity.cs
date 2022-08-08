using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UserPrivilegeRequirementsEntityData))]
	public class UserPrivilegeRequirementsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UserPrivilegeRequirementsEntityData>
	{
		public new FrostySdk.Ebx.UserPrivilegeRequirementsEntityData Data => data as FrostySdk.Ebx.UserPrivilegeRequirementsEntityData;
		public override string DisplayName => "UserPrivilegeRequirements";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UserPrivilegeRequirementsEntity(FrostySdk.Ebx.UserPrivilegeRequirementsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

