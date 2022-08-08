using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientAcceptInviteEntityData))]
	public class ClientAcceptInviteEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientAcceptInviteEntityData>
	{
		public new FrostySdk.Ebx.ClientAcceptInviteEntityData Data => data as FrostySdk.Ebx.ClientAcceptInviteEntityData;
		public override string DisplayName => "ClientAcceptInvite";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientAcceptInviteEntity(FrostySdk.Ebx.ClientAcceptInviteEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

