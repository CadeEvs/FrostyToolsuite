using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JournalDataProviderData))]
	public class JournalDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.JournalDataProviderData>
	{
		public new FrostySdk.Ebx.JournalDataProviderData Data => data as FrostySdk.Ebx.JournalDataProviderData;
		public override string DisplayName => "JournalDataProvider";

		public JournalDataProvider(FrostySdk.Ebx.JournalDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

