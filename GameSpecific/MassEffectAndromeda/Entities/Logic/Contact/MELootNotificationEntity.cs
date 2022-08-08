using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MELootNotificationEntityData))]
	public class MELootNotificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MELootNotificationEntityData>
	{
		public new FrostySdk.Ebx.MELootNotificationEntityData Data => data as FrostySdk.Ebx.MELootNotificationEntityData;
		public override string DisplayName => "MELootNotification";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ItemCount", Direction.Out)
			};
		}

        public MELootNotificationEntity(FrostySdk.Ebx.MELootNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

