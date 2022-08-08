using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UberNotificationEntityData))]
	public class UberNotificationEntity : SubTitledNotificationEntity, IEntityData<FrostySdk.Ebx.UberNotificationEntityData>
	{
		public new FrostySdk.Ebx.UberNotificationEntityData Data => data as FrostySdk.Ebx.UberNotificationEntityData;
		public override string DisplayName => "UberNotification";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ProgressValue", Direction.In),
				new ConnectionDesc("ProgressTotal", Direction.In)
			};
		}

		public UberNotificationEntity(FrostySdk.Ebx.UberNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

