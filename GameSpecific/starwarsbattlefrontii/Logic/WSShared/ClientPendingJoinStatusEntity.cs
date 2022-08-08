using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPendingJoinStatusEntityData))]
	public class ClientPendingJoinStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPendingJoinStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientPendingJoinStatusEntityData Data => data as FrostySdk.Ebx.ClientPendingJoinStatusEntityData;
		public override string DisplayName => "ClientPendingJoinStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPendingJoinStatusEntity(FrostySdk.Ebx.ClientPendingJoinStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

