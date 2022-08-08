using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReceiveEventEntityData))]
	public class ReceiveEventEntity : RemoteEventEntity, IEntityData<FrostySdk.Ebx.ReceiveEventEntityData>
	{
		public new FrostySdk.Ebx.ReceiveEventEntityData Data => data as FrostySdk.Ebx.ReceiveEventEntityData;
		public override string DisplayName => "ReceiveEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ReceiveEventEntity(FrostySdk.Ebx.ReceiveEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

