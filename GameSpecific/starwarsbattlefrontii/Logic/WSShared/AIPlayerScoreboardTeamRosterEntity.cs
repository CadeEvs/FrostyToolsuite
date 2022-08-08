using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPlayerScoreboardTeamRosterEntityData))]
	public class AIPlayerScoreboardTeamRosterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIPlayerScoreboardTeamRosterEntityData>
	{
		public new FrostySdk.Ebx.AIPlayerScoreboardTeamRosterEntityData Data => data as FrostySdk.Ebx.AIPlayerScoreboardTeamRosterEntityData;
		public override string DisplayName => "AIPlayerScoreboardTeamRoster";

		public AIPlayerScoreboardTeamRosterEntity(FrostySdk.Ebx.AIPlayerScoreboardTeamRosterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

