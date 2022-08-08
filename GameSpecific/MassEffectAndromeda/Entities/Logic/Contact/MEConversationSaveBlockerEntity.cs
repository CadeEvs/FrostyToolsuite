using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEConversationSaveBlockerEntityData))]
	public class MEConversationSaveBlockerEntity : SingletonEntity, IEntityData<FrostySdk.Ebx.MEConversationSaveBlockerEntityData>
	{
		public new FrostySdk.Ebx.MEConversationSaveBlockerEntityData Data => data as FrostySdk.Ebx.MEConversationSaveBlockerEntityData;
		public override string DisplayName => "MEConversationSaveBlocker";

		public MEConversationSaveBlockerEntity(FrostySdk.Ebx.MEConversationSaveBlockerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

