using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGroupEventEntityData))]
	public class ClientGroupEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGroupEventEntityData>
	{
		public new FrostySdk.Ebx.ClientGroupEventEntityData Data => data as FrostySdk.Ebx.ClientGroupEventEntityData;
		public override string DisplayName => "ClientGroupEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGroupEventEntity(FrostySdk.Ebx.ClientGroupEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

