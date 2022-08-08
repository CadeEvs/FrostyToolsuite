using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MoverFollowWaypointsEntityData))]
	public class MoverFollowWaypointsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MoverFollowWaypointsEntityData>
	{
		public new FrostySdk.Ebx.MoverFollowWaypointsEntityData Data => data as FrostySdk.Ebx.MoverFollowWaypointsEntityData;
		public override string DisplayName => "MoverFollowWaypoints";

		public MoverFollowWaypointsEntity(FrostySdk.Ebx.MoverFollowWaypointsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

