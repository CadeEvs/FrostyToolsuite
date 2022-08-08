using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIObstacleControllerEntityData))]
	public class AIObstacleControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIObstacleControllerEntityData>
	{
		public new FrostySdk.Ebx.AIObstacleControllerEntityData Data => data as FrostySdk.Ebx.AIObstacleControllerEntityData;
		public override string DisplayName => "AIObstacleController";

		public AIObstacleControllerEntity(FrostySdk.Ebx.AIObstacleControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

