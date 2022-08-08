using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SendEventEntityData))]
	public class SendEventEntity : RemoteEventEntity, IEntityData<FrostySdk.Ebx.SendEventEntityData>
	{
		public new FrostySdk.Ebx.SendEventEntityData Data => data as FrostySdk.Ebx.SendEventEntityData;
		public override string DisplayName => "SendEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SendEventEntity(FrostySdk.Ebx.SendEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

