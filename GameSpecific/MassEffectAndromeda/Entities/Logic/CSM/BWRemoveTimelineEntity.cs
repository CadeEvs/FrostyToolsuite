using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWRemoveTimelineEntityData))]
	public class BWRemoveTimelineEntity : BWApplyTimelineEntityBase, IEntityData<FrostySdk.Ebx.BWRemoveTimelineEntityData>
	{
		public new FrostySdk.Ebx.BWRemoveTimelineEntityData Data => data as FrostySdk.Ebx.BWRemoveTimelineEntityData;
		public override string DisplayName => "BWRemoveTimeline";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BWRemoveTimelineEntity(FrostySdk.Ebx.BWRemoveTimelineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

