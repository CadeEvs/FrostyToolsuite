using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FollowWaypointsEntityData))]
	public class FollowWaypointsEntity : FollowWaypointsEntityBase, IEntityData<FrostySdk.Ebx.FollowWaypointsEntityData>
	{
		public new FrostySdk.Ebx.FollowWaypointsEntityData Data => data as FrostySdk.Ebx.FollowWaypointsEntityData;
		public override string DisplayName => "FollowWaypoints";

		public FollowWaypointsEntity(FrostySdk.Ebx.FollowWaypointsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

