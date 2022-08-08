using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientInviteStatusEntityData))]
	public class ClientInviteStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientInviteStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientInviteStatusEntityData Data => data as FrostySdk.Ebx.ClientInviteStatusEntityData;
		public override string DisplayName => "ClientInviteStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientInviteStatusEntity(FrostySdk.Ebx.ClientInviteStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

