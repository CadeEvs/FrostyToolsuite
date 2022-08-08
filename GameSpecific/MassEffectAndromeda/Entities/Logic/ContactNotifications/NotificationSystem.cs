using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NotificationSystemData))]
	public class NotificationSystem : SingletonEntity, IEntityData<FrostySdk.Ebx.NotificationSystemData>
	{
		public new FrostySdk.Ebx.NotificationSystemData Data => data as FrostySdk.Ebx.NotificationSystemData;
		public override string DisplayName => "NotificationSystem";

		public NotificationSystem(FrostySdk.Ebx.NotificationSystemData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

