using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPrivilegesEntityData))]
	public class ClientPrivilegesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPrivilegesEntityData>
	{
		public new FrostySdk.Ebx.ClientPrivilegesEntityData Data => data as FrostySdk.Ebx.ClientPrivilegesEntityData;
		public override string DisplayName => "ClientPrivileges";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPrivilegesEntity(FrostySdk.Ebx.ClientPrivilegesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

