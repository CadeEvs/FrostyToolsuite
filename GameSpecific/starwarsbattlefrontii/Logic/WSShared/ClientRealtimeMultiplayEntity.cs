using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientRealtimeMultiplayEntityData))]
	public class ClientRealtimeMultiplayEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientRealtimeMultiplayEntityData>
	{
		public new FrostySdk.Ebx.ClientRealtimeMultiplayEntityData Data => data as FrostySdk.Ebx.ClientRealtimeMultiplayEntityData;
		public override string DisplayName => "ClientRealtimeMultiplay";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientRealtimeMultiplayEntity(FrostySdk.Ebx.ClientRealtimeMultiplayEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

