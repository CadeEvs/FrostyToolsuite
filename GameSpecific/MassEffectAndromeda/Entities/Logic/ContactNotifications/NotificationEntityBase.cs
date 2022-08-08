using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NotificationEntityBaseData))]
	public class NotificationEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.NotificationEntityBaseData>
	{
		public new FrostySdk.Ebx.NotificationEntityBaseData Data => data as FrostySdk.Ebx.NotificationEntityBaseData;
		public override string DisplayName => "NotificationEntityBase";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ShowNotification", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OverrideTitleTextStringID", Direction.In)
			};
		}

		public NotificationEntityBase(FrostySdk.Ebx.NotificationEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

