using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationCameraControlNotificationEntityData))]
	public class ConversationCameraControlNotificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConversationCameraControlNotificationEntityData>
	{
		public new FrostySdk.Ebx.ConversationCameraControlNotificationEntityData Data => data as FrostySdk.Ebx.ConversationCameraControlNotificationEntityData;
		public override string DisplayName => "ConversationCameraControlNotification";

		public ConversationCameraControlNotificationEntity(FrostySdk.Ebx.ConversationCameraControlNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

