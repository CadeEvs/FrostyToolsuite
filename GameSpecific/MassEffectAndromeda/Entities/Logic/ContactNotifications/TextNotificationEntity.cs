using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextNotificationEntityData))]
	public class TextNotificationEntity : SubTitledNotificationEntity, IEntityData<FrostySdk.Ebx.TextNotificationEntityData>
	{
		public new FrostySdk.Ebx.TextNotificationEntityData Data => data as FrostySdk.Ebx.TextNotificationEntityData;
		public override string DisplayName => "TextNotification";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TextNotificationEntity(FrostySdk.Ebx.TextNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

