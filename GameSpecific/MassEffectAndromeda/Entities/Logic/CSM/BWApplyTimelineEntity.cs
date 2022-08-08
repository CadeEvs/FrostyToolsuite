using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWApplyTimelineEntityData))]
	public class BWApplyTimelineEntity : BWApplyTimelineEntityBase, IEntityData<FrostySdk.Ebx.BWApplyTimelineEntityData>
	{
		public new FrostySdk.Ebx.BWApplyTimelineEntityData Data => data as FrostySdk.Ebx.BWApplyTimelineEntityData;
		public override string DisplayName => "BWApplyTimeline";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BWApplyTimelineEntity(FrostySdk.Ebx.BWApplyTimelineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

