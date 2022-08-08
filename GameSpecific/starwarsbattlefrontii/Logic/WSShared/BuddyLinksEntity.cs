using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BuddyLinksEntityData))]
	public class BuddyLinksEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BuddyLinksEntityData>
	{
		public new FrostySdk.Ebx.BuddyLinksEntityData Data => data as FrostySdk.Ebx.BuddyLinksEntityData;
		public override string DisplayName => "BuddyLinks";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BuddyLinksEntity(FrostySdk.Ebx.BuddyLinksEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

