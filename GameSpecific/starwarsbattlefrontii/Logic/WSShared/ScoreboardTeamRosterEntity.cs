using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreboardTeamRosterEntityData))]
	public class ScoreboardTeamRosterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScoreboardTeamRosterEntityData>
	{
		public new FrostySdk.Ebx.ScoreboardTeamRosterEntityData Data => data as FrostySdk.Ebx.ScoreboardTeamRosterEntityData;
		public override string DisplayName => "ScoreboardTeamRoster";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ScoreboardTeamRosterEntity(FrostySdk.Ebx.ScoreboardTeamRosterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

