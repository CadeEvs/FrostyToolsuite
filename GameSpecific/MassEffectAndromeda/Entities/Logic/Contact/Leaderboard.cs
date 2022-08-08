using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LeaderboardData))]
	public class Leaderboard : LogicEntity, IEntityData<FrostySdk.Ebx.LeaderboardData>
	{
		public new FrostySdk.Ebx.LeaderboardData Data => data as FrostySdk.Ebx.LeaderboardData;
		public override string DisplayName => "Leaderboard";

		public Leaderboard(FrostySdk.Ebx.LeaderboardData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

