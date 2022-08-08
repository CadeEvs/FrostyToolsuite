using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnBadgeEarnedEntityData))]
	public class OnBadgeEarnedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnBadgeEarnedEntityData>
	{
		public new FrostySdk.Ebx.OnBadgeEarnedEntityData Data => data as FrostySdk.Ebx.OnBadgeEarnedEntityData;
		public override string DisplayName => "OnBadgeEarned";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("BadgeI18nTitle", Direction.Out)
			};
		}

		public OnBadgeEarnedEntity(FrostySdk.Ebx.OnBadgeEarnedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

