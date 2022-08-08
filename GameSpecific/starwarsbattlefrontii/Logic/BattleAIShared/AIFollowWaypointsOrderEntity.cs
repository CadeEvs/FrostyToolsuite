using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIFollowWaypointsOrderEntityData))]
	public class AIFollowWaypointsOrderEntity : AIOrderEntityBase, IEntityData<FrostySdk.Ebx.AIFollowWaypointsOrderEntityData>
	{
		public new FrostySdk.Ebx.AIFollowWaypointsOrderEntityData Data => data as FrostySdk.Ebx.AIFollowWaypointsOrderEntityData;
		public override string DisplayName => "AIFollowWaypointsOrder";

		public AIFollowWaypointsOrderEntity(FrostySdk.Ebx.AIFollowWaypointsOrderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

