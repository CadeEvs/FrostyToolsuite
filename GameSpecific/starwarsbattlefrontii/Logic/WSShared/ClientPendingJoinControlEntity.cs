using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPendingJoinControlEntityData))]
	public class ClientPendingJoinControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPendingJoinControlEntityData>
	{
		public new FrostySdk.Ebx.ClientPendingJoinControlEntityData Data => data as FrostySdk.Ebx.ClientPendingJoinControlEntityData;
		public override string DisplayName => "ClientPendingJoinControl";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPendingJoinControlEntity(FrostySdk.Ebx.ClientPendingJoinControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

