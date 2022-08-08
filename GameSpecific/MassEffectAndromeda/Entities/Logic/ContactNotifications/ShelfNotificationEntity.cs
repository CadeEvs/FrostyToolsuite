using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShelfNotificationEntityData))]
	public class ShelfNotificationEntity : NotificationEntityBase, IEntityData<FrostySdk.Ebx.ShelfNotificationEntityData>
	{
		public new FrostySdk.Ebx.ShelfNotificationEntityData Data => data as FrostySdk.Ebx.ShelfNotificationEntityData;
		public override string DisplayName => "ShelfNotification";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ShelfNotificationEntity(FrostySdk.Ebx.ShelfNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

