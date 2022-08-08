using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathfindingLoadAgentEntityData))]
	public class PathfindingLoadAgentEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PathfindingLoadAgentEntityData>
	{
		public new FrostySdk.Ebx.PathfindingLoadAgentEntityData Data => data as FrostySdk.Ebx.PathfindingLoadAgentEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PathfindingLoadAgentEntity(FrostySdk.Ebx.PathfindingLoadAgentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

