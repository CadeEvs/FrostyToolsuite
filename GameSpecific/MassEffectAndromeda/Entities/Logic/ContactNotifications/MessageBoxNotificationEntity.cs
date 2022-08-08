using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MessageBoxNotificationEntityData))]
	public class MessageBoxNotificationEntity : SubTitledNotificationEntity, IEntityData<FrostySdk.Ebx.MessageBoxNotificationEntityData>
	{
		public new FrostySdk.Ebx.MessageBoxNotificationEntityData Data => data as FrostySdk.Ebx.MessageBoxNotificationEntityData;
		public override string DisplayName => "MessageBoxNotification";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MessageBoxNotificationEntity(FrostySdk.Ebx.MessageBoxNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

