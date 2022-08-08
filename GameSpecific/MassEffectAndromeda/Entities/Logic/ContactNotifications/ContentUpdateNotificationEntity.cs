using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContentUpdateNotificationEntityData))]
	public class ContentUpdateNotificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ContentUpdateNotificationEntityData>
	{
		public new FrostySdk.Ebx.ContentUpdateNotificationEntityData Data => data as FrostySdk.Ebx.ContentUpdateNotificationEntityData;
		public override string DisplayName => "ContentUpdateNotification";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContentUpdateNotificationEntity(FrostySdk.Ebx.ContentUpdateNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

