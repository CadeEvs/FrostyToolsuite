
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BattleAINavObstacleComponentData))]
	public class BattleAINavObstacleComponent : GameComponent, IEntityData<FrostySdk.Ebx.BattleAINavObstacleComponentData>
	{
		public new FrostySdk.Ebx.BattleAINavObstacleComponentData Data => data as FrostySdk.Ebx.BattleAINavObstacleComponentData;
		public override string DisplayName => "BattleAINavObstacleComponent";

		public BattleAINavObstacleComponent(FrostySdk.Ebx.BattleAINavObstacleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

