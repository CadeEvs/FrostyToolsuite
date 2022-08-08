using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PerfJournalBookmarkEntityData))]
	public class PerfJournalBookmarkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PerfJournalBookmarkEntityData>
	{
		public new FrostySdk.Ebx.PerfJournalBookmarkEntityData Data => data as FrostySdk.Ebx.PerfJournalBookmarkEntityData;
		public override string DisplayName => "PerfJournalBookmark";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PerfJournalBookmarkEntity(FrostySdk.Ebx.PerfJournalBookmarkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

