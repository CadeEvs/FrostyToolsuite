using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JournalSetActiveEntityData))]
	public class JournalSetActiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JournalSetActiveEntityData>
	{
		public new FrostySdk.Ebx.JournalSetActiveEntityData Data => data as FrostySdk.Ebx.JournalSetActiveEntityData;
		public override string DisplayName => "JournalSetActive";

		public JournalSetActiveEntity(FrostySdk.Ebx.JournalSetActiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

