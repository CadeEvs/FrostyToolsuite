using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGameQueueStatusEntityData))]
	public class ClientGameQueueStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGameQueueStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientGameQueueStatusEntityData Data => data as FrostySdk.Ebx.ClientGameQueueStatusEntityData;
		public override string DisplayName => "ClientGameQueueStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGameQueueStatusEntity(FrostySdk.Ebx.ClientGameQueueStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

