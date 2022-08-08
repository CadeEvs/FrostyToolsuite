using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientJoinFriendEntityData))]
	public class ClientJoinFriendEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientJoinFriendEntityData>
	{
		public new FrostySdk.Ebx.ClientJoinFriendEntityData Data => data as FrostySdk.Ebx.ClientJoinFriendEntityData;
		public override string DisplayName => "ClientJoinFriend";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientJoinFriendEntity(FrostySdk.Ebx.ClientJoinFriendEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

