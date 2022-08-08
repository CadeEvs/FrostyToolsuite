using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JournalNotificationEntityData))]
	public class JournalNotificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JournalNotificationEntityData>
	{
		public new FrostySdk.Ebx.JournalNotificationEntityData Data => data as FrostySdk.Ebx.JournalNotificationEntityData;
		public override string DisplayName => "JournalNotification";

		public JournalNotificationEntity(FrostySdk.Ebx.JournalNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

