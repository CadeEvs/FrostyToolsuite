using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientFriendsListEntityData))]
	public class ClientFriendsListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientFriendsListEntityData>
	{
		public new FrostySdk.Ebx.ClientFriendsListEntityData Data => data as FrostySdk.Ebx.ClientFriendsListEntityData;
		public override string DisplayName => "ClientFriendsList";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientFriendsListEntity(FrostySdk.Ebx.ClientFriendsListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

