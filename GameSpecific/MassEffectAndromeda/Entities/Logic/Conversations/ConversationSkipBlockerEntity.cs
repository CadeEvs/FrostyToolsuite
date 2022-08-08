using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationSkipBlockerEntityData))]
	public class ConversationSkipBlockerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConversationSkipBlockerEntityData>
	{
		public new FrostySdk.Ebx.ConversationSkipBlockerEntityData Data => data as FrostySdk.Ebx.ConversationSkipBlockerEntityData;
		public override string DisplayName => "ConversationSkipBlocker";

		public ConversationSkipBlockerEntity(FrostySdk.Ebx.ConversationSkipBlockerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

