using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientSendInviteEntityData))]
	public class ClientSendInviteEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientSendInviteEntityData>
	{
		public new FrostySdk.Ebx.ClientSendInviteEntityData Data => data as FrostySdk.Ebx.ClientSendInviteEntityData;
		public override string DisplayName => "ClientSendInvite";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientSendInviteEntity(FrostySdk.Ebx.ClientSendInviteEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

