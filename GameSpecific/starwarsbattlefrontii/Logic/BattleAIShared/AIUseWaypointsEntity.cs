using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIUseWaypointsEntityData))]
	public class AIUseWaypointsEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIUseWaypointsEntityData>
	{
		public new FrostySdk.Ebx.AIUseWaypointsEntityData Data => data as FrostySdk.Ebx.AIUseWaypointsEntityData;
		public override string DisplayName => "AIUseWaypoints";

		public AIUseWaypointsEntity(FrostySdk.Ebx.AIUseWaypointsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

