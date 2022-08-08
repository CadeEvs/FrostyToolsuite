using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObstacleDetectionEntityData))]
	public class ObstacleDetectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObstacleDetectionEntityData>
	{
		public new FrostySdk.Ebx.ObstacleDetectionEntityData Data => data as FrostySdk.Ebx.ObstacleDetectionEntityData;
		public override string DisplayName => "ObstacleDetection";

		public ObstacleDetectionEntity(FrostySdk.Ebx.ObstacleDetectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

