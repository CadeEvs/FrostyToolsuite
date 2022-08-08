using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObstacleDetectionHintData))]
	public class ObstacleDetectionHint : LogicEntity, IEntityData<FrostySdk.Ebx.ObstacleDetectionHintData>
	{
		public new FrostySdk.Ebx.ObstacleDetectionHintData Data => data as FrostySdk.Ebx.ObstacleDetectionHintData;
		public override string DisplayName => "ObstacleDetectionHint";

		public ObstacleDetectionHint(FrostySdk.Ebx.ObstacleDetectionHintData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

