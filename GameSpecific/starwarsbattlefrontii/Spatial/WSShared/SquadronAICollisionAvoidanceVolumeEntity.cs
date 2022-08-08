using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadronAICollisionAvoidanceVolumeEntityData))]
	public class SquadronAICollisionAvoidanceVolumeEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SquadronAICollisionAvoidanceVolumeEntityData>
	{
		public new FrostySdk.Ebx.SquadronAICollisionAvoidanceVolumeEntityData Data => data as FrostySdk.Ebx.SquadronAICollisionAvoidanceVolumeEntityData;

		public SquadronAICollisionAvoidanceVolumeEntity(FrostySdk.Ebx.SquadronAICollisionAvoidanceVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

