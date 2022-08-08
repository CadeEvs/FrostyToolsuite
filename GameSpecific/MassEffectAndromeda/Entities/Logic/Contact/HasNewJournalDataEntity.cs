using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HasNewJournalDataEntityData))]
	public class HasNewJournalDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HasNewJournalDataEntityData>
	{
		public new FrostySdk.Ebx.HasNewJournalDataEntityData Data => data as FrostySdk.Ebx.HasNewJournalDataEntityData;
		public override string DisplayName => "HasNewJournalData";

		public HasNewJournalDataEntity(FrostySdk.Ebx.HasNewJournalDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

