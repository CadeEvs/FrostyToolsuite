using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationWheelUIEntityData))]
	public class ConversationWheelUIEntity : SingletonEntity, IEntityData<FrostySdk.Ebx.ConversationWheelUIEntityData>
	{
		public new FrostySdk.Ebx.ConversationWheelUIEntityData Data => data as FrostySdk.Ebx.ConversationWheelUIEntityData;
		public override string DisplayName => "ConversationWheelUI";

		public ConversationWheelUIEntity(FrostySdk.Ebx.ConversationWheelUIEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

