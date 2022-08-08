using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PerfJournalBookmarkLabelEntityData))]
	public class PerfJournalBookmarkLabelEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PerfJournalBookmarkLabelEntityData>
	{
		public new FrostySdk.Ebx.PerfJournalBookmarkLabelEntityData Data => data as FrostySdk.Ebx.PerfJournalBookmarkLabelEntityData;
		public override string DisplayName => "PerfJournalBookmarkLabel";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PerfJournalBookmarkLabelEntity(FrostySdk.Ebx.PerfJournalBookmarkLabelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

