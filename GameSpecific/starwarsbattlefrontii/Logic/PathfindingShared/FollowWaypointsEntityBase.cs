using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FollowWaypointsEntityBaseData))]
	public class FollowWaypointsEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.FollowWaypointsEntityBaseData>
	{
		public new FrostySdk.Ebx.FollowWaypointsEntityBaseData Data => data as FrostySdk.Ebx.FollowWaypointsEntityBaseData;
		public override string DisplayName => "FollowWaypointsEntityBase";

		public FollowWaypointsEntityBase(FrostySdk.Ebx.FollowWaypointsEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

