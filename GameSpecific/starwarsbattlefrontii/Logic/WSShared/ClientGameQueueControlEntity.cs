using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGameQueueControlEntityData))]
	public class ClientGameQueueControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGameQueueControlEntityData>
	{
		public new FrostySdk.Ebx.ClientGameQueueControlEntityData Data => data as FrostySdk.Ebx.ClientGameQueueControlEntityData;
		public override string DisplayName => "ClientGameQueueControl";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGameQueueControlEntity(FrostySdk.Ebx.ClientGameQueueControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

