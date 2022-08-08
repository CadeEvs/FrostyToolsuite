using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreboardEntityData))]
	public class ScoreboardEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScoreboardEntityData>
	{
		public new FrostySdk.Ebx.ScoreboardEntityData Data => data as FrostySdk.Ebx.ScoreboardEntityData;
		public override string DisplayName => "Scoreboard";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ScoreboardEntity(FrostySdk.Ebx.ScoreboardEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

