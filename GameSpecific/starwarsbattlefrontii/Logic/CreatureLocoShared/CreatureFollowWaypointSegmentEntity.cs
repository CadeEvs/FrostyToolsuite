using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureFollowWaypointSegmentEntityData))]
	public class CreatureFollowWaypointSegmentEntity : CreatureFollowBase, IEntityData<FrostySdk.Ebx.CreatureFollowWaypointSegmentEntityData>
	{
		public new FrostySdk.Ebx.CreatureFollowWaypointSegmentEntityData Data => data as FrostySdk.Ebx.CreatureFollowWaypointSegmentEntityData;
		public override string DisplayName => "CreatureFollowWaypointSegment";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureFollowWaypointSegmentEntity(FrostySdk.Ebx.CreatureFollowWaypointSegmentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

