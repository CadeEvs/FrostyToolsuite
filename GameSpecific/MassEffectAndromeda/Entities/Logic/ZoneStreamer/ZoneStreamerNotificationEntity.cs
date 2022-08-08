using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerNotificationEntityData))]
	public class ZoneStreamerNotificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerNotificationEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerNotificationEntityData Data => data as FrostySdk.Ebx.ZoneStreamerNotificationEntityData;
		public override string DisplayName => "ZoneStreamerNotification";

		public ZoneStreamerNotificationEntity(FrostySdk.Ebx.ZoneStreamerNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

