using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationTimelineEntityData))]
	public class ConversationTimelineEntity : TimelineEntity, IEntityData<FrostySdk.Ebx.ConversationTimelineEntityData>
	{
		public new FrostySdk.Ebx.ConversationTimelineEntityData Data => data as FrostySdk.Ebx.ConversationTimelineEntityData;
		public override string DisplayName => "ConversationTimeline";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConversationTimelineEntity(FrostySdk.Ebx.ConversationTimelineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

