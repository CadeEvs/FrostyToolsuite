using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceBattleDistanceLogicData))]
	public class SpaceBattleDistanceLogic : LogicEntity, IEntityData<FrostySdk.Ebx.SpaceBattleDistanceLogicData>
	{
		public new FrostySdk.Ebx.SpaceBattleDistanceLogicData Data => data as FrostySdk.Ebx.SpaceBattleDistanceLogicData;
		public override string DisplayName => "SpaceBattleDistanceLogic";

		public SpaceBattleDistanceLogic(FrostySdk.Ebx.SpaceBattleDistanceLogicData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

