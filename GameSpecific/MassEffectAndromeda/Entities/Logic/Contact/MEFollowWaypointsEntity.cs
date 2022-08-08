using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEFollowWaypointsEntityData))]
	public class MEFollowWaypointsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEFollowWaypointsEntityData>
	{
		public new FrostySdk.Ebx.MEFollowWaypointsEntityData Data => data as FrostySdk.Ebx.MEFollowWaypointsEntityData;
		public override string DisplayName => "MEFollowWaypoints";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Follower", Direction.In),
				new ConnectionDesc("Waypoints", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Start", Direction.In),
				new ConnectionDesc("Stop", Direction.In),
				new ConnectionDesc("OnComplete", Direction.Out)
			};
		}

		public MEFollowWaypointsEntity(FrostySdk.Ebx.MEFollowWaypointsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

