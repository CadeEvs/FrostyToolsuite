using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MatchmakingScenarioInfoEntityData))]
	public class MatchmakingScenarioInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MatchmakingScenarioInfoEntityData>
	{
		public new FrostySdk.Ebx.MatchmakingScenarioInfoEntityData Data => data as FrostySdk.Ebx.MatchmakingScenarioInfoEntityData;
		public override string DisplayName => "MatchmakingScenarioInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MatchmakingScenarioInfoEntity(FrostySdk.Ebx.MatchmakingScenarioInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

