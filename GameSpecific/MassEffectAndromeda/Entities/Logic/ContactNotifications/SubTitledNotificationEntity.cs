using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubTitledNotificationEntityData))]
	public class SubTitledNotificationEntity : NotificationEntityBase, IEntityData<FrostySdk.Ebx.SubTitledNotificationEntityData>
	{
		public new FrostySdk.Ebx.SubTitledNotificationEntityData Data => data as FrostySdk.Ebx.SubTitledNotificationEntityData;
		public override string DisplayName => "SubTitledNotification";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("OverrideSubTextStringID", Direction.In));
				return outProperties;
			}
		}

		public SubTitledNotificationEntity(FrostySdk.Ebx.SubTitledNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

