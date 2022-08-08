using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NotificationsDataProviderData))]
	public class NotificationsDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.NotificationsDataProviderData>
	{
		public new FrostySdk.Ebx.NotificationsDataProviderData Data => data as FrostySdk.Ebx.NotificationsDataProviderData;
		public override string DisplayName => "NotificationsDataProvider";

		public NotificationsDataProvider(FrostySdk.Ebx.NotificationsDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

