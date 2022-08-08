using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FollowWaypointsManeuverEntityData))]
	public class FollowWaypointsManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.FollowWaypointsManeuverEntityData>
	{
		public new FrostySdk.Ebx.FollowWaypointsManeuverEntityData Data => data as FrostySdk.Ebx.FollowWaypointsManeuverEntityData;
		public override string DisplayName => "FollowWaypointsManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FollowWaypointsManeuverEntity(FrostySdk.Ebx.FollowWaypointsManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

